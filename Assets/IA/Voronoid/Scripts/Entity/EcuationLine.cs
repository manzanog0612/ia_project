using UnityEngine;

namespace IA.Voronoid.Entity
{
    public struct EcuationLine
    {
        public enum Y_INTERCEPT_TYPE { ONE_VALUE, ALL, NONE  }

        #region PRIVATE_FIELDS
        private float slope;
        private float yIntercept;
        private Y_INTERCEPT_TYPE yInterceptType;

        public float a;
        public float b;
        public float c;
        #endregion

        #region PROPERTIES
        public float YIntercept { get => yIntercept; }
        public float Slope { get => slope; }
        public Y_INTERCEPT_TYPE YInterceptType { get => yInterceptType; set => yInterceptType = value; }
        #endregion

        #region CONSTRUCTOR
        public EcuationLine(Vector2 p1, Vector2 p2)
        {
            yInterceptType = Y_INTERCEPT_TYPE.ONE_VALUE;
            slope = (p1.x - p2.x) == 0 ? 0 : (p1.y - p2.y) / (p1.x - p2.x);
            yIntercept = p1.y - slope * p1.x;

            a = slope;
            b = -1;
            c = p1.y - slope * p1.x;
            //if (slope == 0)
            //{
            //    yIntercept = 0;
            //
            //    if (p1.x - p2.x == 0)
            //    {
            //        if (p1.x == 0)
            //        {
            //            yInterceptType = Y_INTERCEPT_TYPE.ALL;
            //        }
            //        else
            //        {
            //            yInterceptType = Y_INTERCEPT_TYPE.NONE;
            //        }
            //    }
            //}
            //else
            //{
            //    yIntercept = p1.y - slope * p1.x;
            //}
        }
        #endregion
    }
}
