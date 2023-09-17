using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using IA.Voronoid.Entity;

namespace IA.Voronoid.Generator
{
    public class VoronoidGenerator
    {
        #region PRIVATE_FIELDS
        private Vector2Int gridSize = default;

        private List<Limit> limits = null;
        private List<Sector> sectors = new List<Sector>();
        #endregion

        #region PUBLIC_METHODS
        public void Configure(Vector2Int[] points, Vector2Int gridSize, int[,] weigths)
        {
            this.gridSize = gridSize;

            InitLimits();

            sectors.Clear();

            for (int i = 0; i < points.Length; i++)
            {
                sectors.Add(new Sector(points[i]));
                sectors[i].SetSegmentLimits(limits);
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
            SetSectorsWeights(weigths);
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
                new Limit(new Vector2(0, gridSize.y - 1), DIRECTION.UP),
                new Limit(new Vector2(gridSize.x - 1, gridSize.y - 1), DIRECTION.RIGHT),
                new Limit(new Vector2(gridSize.x - 1, 0), DIRECTION.DOWN)
            };
        }

        private int GetTotalWeightOnSector(Sector sector, int[,] weigths)
        {
            int width = weigths.GetLength(0);
            int height = weigths.GetLength(1);

            int totalWeigth = 0;

            Vector2Int aux = new Vector2Int();  
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    aux.x = x;
                    aux.y = y;

                    if (sector.Position != aux && sector.CheckPointInSector(aux))
                    {
                        totalWeigth += weigths[x, y];
                    }                    
                }
            }

             return totalWeigth;
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
        
        private void SetSectorsWeights(int[,] weigths)
        {
            const int amountOfIterations = 7;            
            for (int iterations = 0; iterations < amountOfIterations; iterations++)
            {
                for (int i = 0; i < sectors.Count; i++)
                {
                    for (int j = 0; j < sectors[i].Neighbours.Count; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        Sector sector = sectors[i];
                        Sector neighbour = sectors[i].Neighbours[j];

                        int totalSectorWeigth = GetTotalWeightOnSector(sector, weigths);
                        int totalNeighbourWeigth = GetTotalWeightOnSector(neighbour, weigths);

                        float sectorWeightPercentage = totalSectorWeigth / (float)(totalSectorWeigth + totalNeighbourWeigth);
                        float neighbourWeightPercentage = totalNeighbourWeigth / (float)(totalSectorWeigth + totalNeighbourWeigth);

                        BalanceSector(sector, neighbour, 1 - sectorWeightPercentage);
                        BalanceSector(neighbour, sector, 1 - neighbourWeightPercentage);
                    }
                }

                //for (int i = 0; i < sectors.Count; i++)
                //{
                //    sectors[i].Neighbours.Clear();
                //}
                //
                //SetSectorsNeighbours();
            }
        }

        private void BalanceSector(Sector sector, Sector neighbour, float sectorWeightPercentage)
        {
            Segment sectorSegment = sector.GetSegmentOfSector(neighbour);

            if (sectorSegment == null)
            {
                return;
            }

            sectorSegment.SetMediatrixByPercentage(sectorWeightPercentage);
            sector.SetIntersections(gridSize);
        }
        #endregion
    }
}