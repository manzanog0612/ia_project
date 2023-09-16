using System.Collections.Generic;

using UnityEngine;

using IA.Voronoid.Entity;

using Grid = IA.Pathfinding.Grid;
using System.Linq;

namespace IA.Voronoid.Generator
{
    public class VoronoidGenerator
    {
        #region PRIVATE_FIELDS
        private Vector2Int[] points = null;
        private Vector2Int gridSize = default;

        private List<Limit> limits = null;
        private List<Sector> sectors = new List<Sector>();
        #endregion

        #region PUBLIC_METHODS
        public void Configure(Vector2Int[] points, int[,] weigths)
        {
            this.points = points;

            InitLimits();

            sectors.Clear();

            for (int i = 0; i < points.Length; i++)
            {
                sectors.Add(new Sector(points[i]));
                sectors[i].AddSegmentLimits(limits);
            }

            for (int i = 0; i < sectors.Count; i++)
            {
                for (int j = 0; j < sectors.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    sectors[i].AddSegment(sectors[i].Position, sectors[j].Position);
                }
            }

            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].SetIntersections(gridSize);
            }

            SetSectorsNeighbours();
        }

        public void Draw()
        {
            if (sectors == null)
            { 
                return; 
            }

            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].DrawSector();
                sectors[i].DrawSegments();
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitLimits()
        {
            limits = new List<Limit>
            {
                new Limit(Vector2.zero, DIRECTION.LEFT),
                new Limit(new Vector2(0f, gridSize.y - 1), DIRECTION.UP),
                new Limit(new Vector2(gridSize.x - 1, gridSize.y - 1), DIRECTION.RIGHT),
                new Limit(new Vector2(gridSize.x - 1, 0f), DIRECTION.DOWN)
            };
        }

        private void SetSectorsNeighbours()
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                List<Sector> neighbours = new List<Sector>();

                for (int j = 0; j < sectors.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    for (int k = 0; k < sectors[j].Intersections.Count; k++)
                    {
                        if (sectors[i].Intersections.Contains(sectors[j].Intersections[k]))
                        {
                            neighbours.Add(sectors[j]);
                            break;
                        }
                    }
                }

                neighbours.Distinct();

                sectors[i].SetNeighbours(neighbours);
            }
        }

        //pri
        #endregion
    }
}