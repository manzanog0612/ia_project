using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using IA.Voronoid.Entity;

namespace IA.Voronoid.Generator
{
    public class VoronoidGenerator
    {
        #region PRIVATE_FIELDS
        private Vector2 gridSize = default;

        private List<Limit> limits = null;
        private List<Sector> sectors = new List<Sector>();
        #endregion

        #region PUBLIC_METHODS
        public void Configure(Vector2[] points, Vector2 gridSize, int[,] weigths)
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
            //SetSectorsWeights(weigths);
        }

        public Vector2 GetSectorCloserToPosition(Vector2 position)
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                if (sectors[i].CheckPointInSector(position))
                {
                    return sectors[i].Position;
                }
            }

            return Vector2.zero;
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
                new Limit(new Vector2(0, gridSize.y), DIRECTION.UP),
                new Limit(new Vector2(gridSize.x, gridSize.y), DIRECTION.RIGHT),
                new Limit(new Vector2(gridSize.x, 0), DIRECTION.DOWN)
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
                    //calculate total sector weigth percentage
                    Sector sector = sectors[i]; 

                    float sectorWeigth = GetTotalWeightOnSector(sector, weigths);
                    float totalWeight = sectorWeigth;

                    List<Sector> neighbours = sectors[i].Neighbours;
                    for (int j = 0; j < neighbours.Count; j++)
                    {
                        Sector neighbour = neighbours[j];
                        totalWeight += GetTotalWeightOnSector(neighbour, weigths);
                    }

                    float sectorWeightPercentage = sectorWeigth / (float)totalWeight;

                    //balance mediatrices by percentage for all sector's mediatrices and only the modified segment for the neighbours
                    BalanceSector(sector, 1 - sectorWeightPercentage);

                    for (int j = 0; j < neighbours.Count; j++)
                    {
                        Sector neighbour = neighbours[j];
                        
                        BalanceNeighbour(neighbour, sector, sectorWeightPercentage);
                    }

                    //recalculate intersections for sector
                    ResetSector(sector);
                    sector.SetIntersections(gridSize);

                    //once I have the new intersections generate the new mediatrices for all neighbours
                    //first, get the intersection and the segment
                    for (int j = 0; j < neighbours.Count; j++)
                    {
                        Sector neighbour = neighbours[j];

                        Segment neighbourSectorSegment = neighbour.GetSegmentOfSector(sector);
                        Segment sectorSegment = sector.GetSegmentOfSector(neighbour);

                        List<Sector> commonNeighbours = sector.Neighbours.FindAll(s => neighbour.Neighbours.Contains(s));

                        if (sectorSegment == null)
                        {
                            continue;
                        }

                        for (int k = 0; k < commonNeighbours.Count; k++)
                        {
                            //find old intersection
                            Sector commonNeighbour = commonNeighbours[k];
                            Segment commonNeighbourSectorSegmentForNeighbour = neighbour.GetSegmentOfSector(commonNeighbour);

                            if (neighbourSectorSegment == null || neighbourSectorSegment.Intersections == null || commonNeighbourSectorSegmentForNeighbour == null || commonNeighbourSectorSegmentForNeighbour.Intersections == null)
                            {
                                continue;
                            }

                            Vector2 oldIntersection = Vector2.one * -1;

                            for (int l = 0; l < neighbourSectorSegment.Intersections.Count; l++)
                            {
                                if (commonNeighbourSectorSegmentForNeighbour.Intersections.Contains(neighbourSectorSegment.Intersections[l]))
                                {
                                    oldIntersection = neighbourSectorSegment.Intersections[l];
                                }
                            }

                            //Vector2 oldIntersection = neighbourSectorSegment.Intersections.Find(i => commonNeighbourSectorSegmentForNeighbour.Intersections.Contains(i));

                            if (oldIntersection == Vector2.one * -1)
                            {
                                continue;
                            }

                            //find new intersection
                            Segment commonNeighbourSectorSegmentForSector = sector.GetSegmentOfSector(commonNeighbour);

                            if (commonNeighbourSectorSegmentForSector == null)
                            {
                                continue;
                            }

                            Vector2 newIntersection = sectorSegment.Intersections.Find(i => commonNeighbourSectorSegmentForSector.Intersections.Contains(i));

                            //calculate new mediatrix for neighbour and common neighbour though finding the intersection between the common neighbour segment
                            //and a mock segment that follows the same direction as the mediatrix placed on the new intersection point

                            //mediatrix = (origin + end) / 2;
                            //direction = Vector2.Perpendicular(end - origin).normalized;

                            Segment mockSegment1 = commonNeighbourSectorSegmentForNeighbour;
                            Segment mockSegment2 = new Segment(newIntersection - (mockSegment1.Mediatrix + mockSegment1.Direction.normalized * gridSize.magnitude),
                                                               newIntersection + (mockSegment1.Mediatrix + mockSegment1.Direction.normalized * gridSize.magnitude));

                            Vector2? newMediatrix = GetIntersection(mockSegment1, mockSegment2, gridSize);

                            //set found mediatrix for sector's neighbour and common neighbour
                            commonNeighbourSectorSegmentForNeighbour.SetMediatrix(newMediatrix.Value);

                            Segment commonNeighbourSegment = commonNeighbour.GetSegmentOfSector(neighbour);

                            if (commonNeighbourSegment == null)
                            {
                                continue;
                            }

                            commonNeighbourSegment.SetMediatrix(newMediatrix.Value);
                        }
                    }

                    for (int j = 0; j < neighbours.Count; j++)
                    {
                        Sector neighbour = neighbours[j];
                        ResetSector(neighbour);
                        neighbour.SetIntersections(gridSize);
                    }
                }
            }
        }

        private Vector2? GetIntersection(Segment seg1, Segment seg2, Vector2 gridSize)
        {
            Vector2 intersection = Vector2.zero;

            Vector2 p1s = seg1.Mediatrix;
            Vector2 p1e = seg1.Mediatrix + seg1.Direction * gridSize.magnitude;
            Vector2 p2s = seg2.Mediatrix;
            Vector2 p2e = seg2.Mediatrix + seg2.Direction * gridSize.magnitude;

            //cross product to know if segments are paralel
            if (((p1s.x - p1e.x) * (p2s.y - p2e.y) - (p1s.y - p1e.y) * (p2s.x - p2e.x)) == 0)
            {
                return null;
            }

            float p1CrossDiff = p1s.x * p1e.y - p1s.y * p1e.x; //cross difference
            float p2CrossDiff = p2s.x * p2e.y - p2s.y * p2e.x;
            float p2XLength = p2s.x - p2e.x;
            float p2YLength = p2s.y - p2e.y;
            float p1XLength = p1s.x - p1e.x;
            float p1YLength = p1s.y - p1e.y;

            intersection.x = (p1CrossDiff * p2XLength - p1XLength * p2CrossDiff) / (p1XLength * p2YLength - p1YLength * p2XLength);
            intersection.y = (p1CrossDiff * p2YLength - p1YLength * p2CrossDiff) / (p1XLength * p2YLength - p1YLength * p2XLength);

            return intersection;
        }

        private void ResetSector(Sector sector)
        {
            sector.SetSegmentLimits(limits);

            for (int i = 0; i < sectors.Count; i++)
            {
                if (sectors[i] != sector)
                {
                    continue;
                }

                sector.AddSegment(sector.Position, sectors[i].Position);
            }
        }

        private void BalanceSector(Sector sector, float sectorWeightPercentage)
        {
            sector.SetAllSegmentsMatrices(sectorWeightPercentage);
        }

        private void BalanceNeighbour(Sector sector, Sector neighbour, float sectorWeightPercentage)
        {
            Segment sectorSegment = sector.GetSegmentOfSector(neighbour);

            if (sectorSegment == null)
            {
                return;
            }

            sectorSegment.SetMediatrixByPercentage(sectorWeightPercentage);
        }
        #endregion
    }
}