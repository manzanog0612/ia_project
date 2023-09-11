using System;
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
        public void AddSegmentLimits(Vector3[] gridLimits)
        {
            segments.Add(new Segment(gridLimits[0], gridLimits[1]));
            segments.Add(new Segment(gridLimits[1], gridLimits[2]));
            segments.Add(new Segment(gridLimits[2], gridLimits[3]));
            segments.Add(new Segment(gridLimits[3], gridLimits[0]));
        }

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

                    bool checkValidPoint = false;
                    for (int k = 0; k < segments.Count; k++)
                    {
                        if (k == i || k == j) continue;

                        if (CheckIfHaveAnotherPositionCloser(intersection.Value, segments[k].End, maxDistance))
                        {
                            checkValidPoint = true;
                            break;
                        }
                    }

                    if (!checkValidPoint)
                    {
                        intersections.Add(intersection.Value);
                        segments[i].Intersections.Add(intersection.Value);
                        segments[j].Intersections.Add(intersection.Value);
                    }
                }
            }

            segments.RemoveAll((s) => s.Intersections.Count != 2);

            SortIntersections();
            SetPointsInSector();
        }

        #endregion

        #region PRIVATE_METHODS
        private bool CheckIfHaveAnotherPositionCloser(Vector2 intersectionPoint, Vector2 pointEnd, float maxDistance)
        {
            float distance = Vector2.Distance(intersectionPoint, pointEnd);
            return distance < maxDistance;
        }


        public Vector2 GetIntersection(Segment seg1, Segment seg2, Vector2Int gridSize)
        {
            Vector2 intersection = Vector2.zero;
        
            Vector2 p1s = seg1.Mediatrix;
            Vector2 p1e = seg1.Mediatrix + seg1.Direction * gridSize.magnitude;
            Vector2 p2s = seg2.Mediatrix;
            Vector2 p2e = seg2.Mediatrix + seg2.Direction * gridSize.magnitude;

            //cross product to know if segments are paralel
            if (((p1s.x - p1e.x) * (p2s.y - p2e.y) - (p1s.y - p1e.y) * (p2s.x - p2e.x)) == 0) 
            { 
                return intersection; 
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

        private Vector2? GetIntersection2(Segment seg1, Segment seg2, Vector2Int gridSize)
        {
            bool noIntersectionFound = false;
            Vector2 intersection = Vector2.zero;
        
            EcuationLine seg1EcuationLine = seg1.GetEcuationLine(gridSize.magnitude);
            EcuationLine seg2EcuationLine = seg2.GetEcuationLine(gridSize.magnitude);



            //Y_INTERCEPT_TYPE seg1IT = seg1EcuationLine.YInterceptType;
            //Y_INTERCEPT_TYPE seg2IT = seg2EcuationLine.YInterceptType;
            //
            //if (seg1EcuationLine.Slope == 0 || seg2EcuationLine.Slope == 0)
            //{
            //    if (seg1IT == Y_INTERCEPT_TYPE.ALL)
            //    {
            //        switch (seg2IT)
            //        {
            //            case Y_INTERCEPT_TYPE.ONE_VALUE:
            //                intersection.x = (seg1EcuationLine.YIntercept - seg2EcuationLine.YIntercept) / (seg2EcuationLine.Slope - seg1EcuationLine.Slope);
            //                intersection.y = 0;
            //                break;
            //            case Y_INTERCEPT_TYPE.ALL:
            //                intersection = seg1.Mediatrix; //idk
            //                break;
            //            case Y_INTERCEPT_TYPE.NONE:
            //                noIntersectionFound = true;
            //                break;
            //        }
            //    }
            //    else //NONE
            //    {
            //        switch (seg2IT)
            //        {
            //            case Y_INTERCEPT_TYPE.ONE_VALUE:
            //                intersection.x = (seg1EcuationLine.YIntercept - seg2EcuationLine.YIntercept) / (seg2EcuationLine.Slope - seg1EcuationLine.Slope);
            //                intersection.y = 0;
            //                break;
            //            case Y_INTERCEPT_TYPE.ALL:
            //                intersection.x = (seg1EcuationLine.YIntercept - seg2EcuationLine.YIntercept) / (seg2EcuationLine.Slope - seg1EcuationLine.Slope);
            //                intersection.y = 0;
            //                break;
            //            case Y_INTERCEPT_TYPE.NONE:
            //                noIntersectionFound = true;
            //                break;
            //        }
            //    }
            //}
            //else
            //{
            //    intersection.x = (seg1EcuationLine.YIntercept - seg2EcuationLine.YIntercept) / (seg2EcuationLine.Slope - seg1EcuationLine.Slope);
            //    intersection.y = seg1EcuationLine.Slope * intersection.x + seg1EcuationLine.YIntercept;
            //
            /*

            To find X:

            ● m1 * x + b1 = m2 * x + b2
            ● m1 * x + b1 - b2 = m2 * x
            ● b1 - b2 = m2 * x - m1 * x
            ● b1 - b2 = (m2 - m1) * x
            ● (b1 - b2) / (m2 - m1) = x

            To find Y:

            ● y = m1 * x + b1

            */
            //}


            //if (Math.Abs(seg1EcuationLine.Slope - seg2EcuationLine.Slope) < double.Epsilon)
            //{
            //    if (Math.Abs(seg1EcuationLine.YIntercept - seg2EcuationLine.YIntercept) < double.Epsilon)
            //    {
            //        intersection.y = seg1EcuationLine.YIntercept;
            //    }
            //    else
            //    {
            //        noIntersectionFound = true;
            //    }
            //}
            //else
            //{
            //
            //    intersection.x = (seg2EcuationLine.YIntercept - seg1EcuationLine.YIntercept) / (seg1EcuationLine.Slope - seg2EcuationLine.Slope);
            //    intersection.y = seg1EcuationLine.Slope * intersection.x + seg1EcuationLine.YIntercept;
            //}

            intersection.x = (seg1EcuationLine.b * seg2EcuationLine.c - seg2EcuationLine.b * seg1EcuationLine.c) / (-seg1EcuationLine.a * seg2EcuationLine.b - seg2EcuationLine.a * seg1EcuationLine.b);
            intersection.y = (seg1EcuationLine.c * seg2EcuationLine.a - seg1EcuationLine.a * seg2EcuationLine.c) / (seg1EcuationLine.a * seg2EcuationLine.b - seg2EcuationLine.a * seg1EcuationLine.b);

            return noIntersectionFound ? null : intersection;
        }

        private void SortIntersections()
        {
            List<IntersectionPoint> intersectionPoints = new List<IntersectionPoint>();
            for (int i = 0; i < intersections.Count; i++)
            {
                intersectionPoints.Add(new IntersectionPoint(intersections[i]));
            }

            float minX = intersectionPoints[0].Position.x;
            float maxX = intersectionPoints[0].Position.x;
            float minY = intersectionPoints[0].Position.y;
            float maxY = intersectionPoints[0].Position.y;

            for (int i = 0; i < intersections.Count; i++)
            {
                if (intersectionPoints[i].Position.x < minX) minX = intersectionPoints[i].Position.x;
                if (intersectionPoints[i].Position.x > maxX) maxX = intersectionPoints[i].Position.x;
                if (intersectionPoints[i].Position.y < minY) minY = intersectionPoints[i].Position.y;
                if (intersectionPoints[i].Position.y > maxY) maxY = intersectionPoints[i].Position.y;
            }

            Vector2 center = new Vector2(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);

            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                Vector2 pos = intersectionPoints[i].Position;

                intersectionPoints[i].Angle = Mathf.Acos((pos.x - center.x) /
                    Mathf.Sqrt(Mathf.Pow(pos.x - center.x, 2f) + Mathf.Pow(pos.y - center.y, 2f)));

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