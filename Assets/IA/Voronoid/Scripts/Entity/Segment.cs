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

        private List<Vector2> intersections = new List<Vector2>();
        #endregion

        #region PROPERTIES
        public Vector2 Origin { get => origin; }
        public Vector2 End { get => end; }
        public Vector2 Mediatrix { get => mediatrix; }
        public Vector2 Direction { get => direction; }
        public List<Vector2> Intersections { get => intersections; }
        #endregion

        #region CONSTRUCTORS
        public Segment(Vector2 origin, Vector2 end)
        {
            this.origin = origin;
            this.end = end;

            mediatrix = new Vector2((origin.x + end.x) / 2, (origin.y + end.y) / 2);
            direction = Vector2.Perpendicular(new Vector2(end.x - origin.x, end.y - origin.y));

        }
        #endregion

        #region PUBLIC_METHODS
        public EcuationLine GetEcuationLine(float lineLength)
        {
            Vector2 p1 = mediatrix - direction * lineLength;
            Vector2 p2 = mediatrix + direction * lineLength;
            return new EcuationLine(p1, p2);
        }

        public void Draw()
        {
            Gizmos.DrawLine(origin, end);
        }
        #endregion
    }
}