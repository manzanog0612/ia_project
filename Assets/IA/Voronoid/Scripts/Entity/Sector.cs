using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

namespace IA.Voronoid.Entity
{
    public class Sector
    {
        #region PRIVATE_FIELDS
        private Color color = Color.white;
        private List<Segment> segments = null;
        private List<Vector2> intersections = null;
        private Vector3[] points = null;
        private Vector3 position = Vector3.zero;
        #endregion

        #region PROPERTIES
        public Vector3 Position => position;
        #endregion

        #region CONSTRUCTORS
        public Sector(Vector3 position)
        {
            this.position = position;

            color = Random.ColorHSV();
            color.a = 0.35f;

            segments = new List<Segment>();
            intersections = new List<Vector2>();
        }
        #endregion

        #region PUBLIC_METHODS
        public void AddSegmentLimits(List<Limit> limits)
        {
            for (int i = 0; i < limits.Count; i++)
            {
                Vector2 origin = position;
                Vector2 final = limits[i].GetOutsitePosition(origin);

                segments.Add(new Segment(origin, final));
            }
        }

        public void AddSegment(Vector2 origin, Vector2 final)
        {
            segments.Add(new Segment(origin, final));
        }

        public void DrawSegments()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].Draw();
            }
        }

        public void DrawSector()
        {
            Handles.color = color;
            Handles.DrawAAConvexPolygon(points);

            Handles.color = Color.black;
            Handles.DrawPolyLine(points);
        }

        public void SetIntersections(Vector2Int gridSize)
        {
            intersections.Clear();

            for (int i = 0; i < segments.Count; i++)
            {
                for (int j = 0; j < segments.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Vector2? intersection = GetIntersection(segments[i], segments[j], gridSize);

                    if (!intersection.HasValue || intersections.Contains(intersection.Value))
                    {
                        continue;
                    }

                    float maxDistance = Vector2.Distance(intersection.Value, segments[i].Origin);

                    // This check is because if any of the points that aren't in the calculation (the points of the segments, origin and end)
                    // are closer to any of the other points it means that there is a closer intersection that has been calculated yet or it
                    // needs to be calculated so, if we find a smaller distance, we flag the found intersection as invalid
                    bool checkValidIntersection = true;
                    for (int k = 0; k < segments.Count; k++)
                    {
                        if (k == i || k == j)
                        { 
                            continue; 
                        }

                        if (Vector2.Distance(intersection.Value, segments[k].End) < maxDistance)
                        {
                            checkValidIntersection = false;
                            break;
                        }
                    }

                    if (checkValidIntersection)
                    {
                        intersections.Add(intersection.Value);
                        segments[i].Intersections.Add(intersection.Value);
                        segments[j].Intersections.Add(intersection.Value);
                    }
                }
            }

            segments.RemoveAll((s) => s.Intersections.Count != 2);

            SortIntersectionsByAngle();
            SetPointsInSector();
        }

        //https://www.youtube.com/watch?v=RSXM9bgqxJM
        public bool CheckPointInSector(Vector3 point)
        {
            if (points == null)
            { 
                return false; 
            }

            int edges = 0;
            Vector2 end = points[^1];
            
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 start = end;
                end = points[i];

                if ((point.y < start.y) ^ (point.y < end.y) &&
                     point.x < start.x + ((point.y - start.y) / (end.y - start.y)) * (end.x - start.x))
                {
                    edges++;
                }
            }

            return edges % 2 == 1;
        }
        #endregion

        #region PRIVATE_METHODS
        public Vector2? GetIntersection(Segment seg1, Segment seg2, Vector2Int gridSize)
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

        private void SortIntersectionsByAngle()
        {
            List<IntersectionPoint> intersectionPoints = new List<IntersectionPoint>();
            for (int i = 0; i < intersections.Count; i++)
            {
                intersectionPoints.Add(new IntersectionPoint(intersections[i]));
            }

            float minX = intersectionPoints[0].Position.x;
            float minY = intersectionPoints[0].Position.y;
            float maxX = minX;
            float maxY = minY;

            for (int i = 0; i < intersections.Count; i++)
            {
                if (intersectionPoints[i].Position.x < minX)
                {
                    minX = intersectionPoints[i].Position.x;
                }
                if (intersectionPoints[i].Position.x > maxX)
                {
                    maxX = intersectionPoints[i].Position.x;
                }
                if (intersectionPoints[i].Position.y < minY)
                {
                    minY = intersectionPoints[i].Position.y;
                }
                if (intersectionPoints[i].Position.y > maxY) 
                {
                    maxY = intersectionPoints[i].Position.y;
                }
            }

            Vector2 center = new Vector2(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);

            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                Vector2 pos = intersectionPoints[i].Position;
                Vector2 pointDirVector = pos - center;
                //                                   __________
                //                            x / -l/ px² + py²
                intersectionPoints[i].Angle = Mathf.Acos(pointDirVector.x / Mathf.Sqrt(Mathf.Pow(pointDirVector.x, 2f) + Mathf.Pow(pointDirVector.y, 2f)));

                if (pos.y > center.y)
                {
                    intersectionPoints[i].Angle = Mathf.PI + Mathf.PI - intersectionPoints[i].Angle;
                }
            }

            intersectionPoints = intersectionPoints.OrderBy(p => p.Angle).ToList();

            intersections.Clear();
            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                intersections.Add(intersectionPoints[i].Position);
            }
        }

        private void SetPointsInSector()
        {
            points = new Vector3[intersections.Count + 1];

            for (int i = 0; i < intersections.Count; i++)
            {
                points[i] = new Vector3(intersections[i].x, intersections[i].y, 0f);
            }
            points[intersections.Count] = points[0];
        }
        #endregion
    }
}