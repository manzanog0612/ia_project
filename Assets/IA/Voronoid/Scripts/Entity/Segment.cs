using System.Collections.Generic;

using UnityEngine;

namespace IA.Voronoid.Entity
{
    public class Segment
    {
        #region PRIVATE_FIELDS
        private Vector2 origin = Vector2.zero;
        private Vector2 end = Vector2.zero;
        private Vector2 direction = Vector2.zero;
        private Vector2 mediatrix = Vector2.zero;
        private bool isLimit = false;

        private List<Vector2> intersections = new List<Vector2>();
        #endregion

        #region PROPERTIES
        public Vector2 Origin { get => origin; }
        public Vector2 End { get => end; }
        public Vector2 Mediatrix { get => mediatrix; }
        public Vector2 Direction { get => direction; }
        public List<Vector2> Intersections { get => intersections; }
        public bool IsLimit { get => isLimit; }
        #endregion

        #region CONSTRUCTORS
        public Segment(Vector2 origin, Vector2 end, bool isLimit = false)
        {
            this.origin = origin;
            this.end = end;
            this.isLimit = isLimit;

            mediatrix = (origin + end) / 2;
            direction = Vector2.Perpendicular(end - origin).normalized;
        }

        public Segment(Vector2 origin, Vector2 end, Vector2 mediatrix, bool isLimit = false)
        {
            this.origin = origin;
            this.end = end;
            this.isLimit = isLimit;
            this.mediatrix = mediatrix;

            direction = Vector2.Perpendicular(end - origin).normalized;
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetMediatrixByPercentage(float percentage)
        {
            mediatrix = Vector2.Lerp(origin, end, percentage);
        }

        public void Draw()
        {
            Gizmos.DrawLine(origin, end);
        }
        #endregion
    }
}