using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class PlaneRect
    {
        private Vector3 p00;
        private Vector3 p01;
        private Vector3 p10;
        private Vector3 p11;

        #region Constructors
        public PlaneRect()
        {
            p00 = Vector3.zero;
            p01 = new Vector3(1, 0);
            p10 = new Vector3(0, 1);
            p11 = new Vector3(1, 1);
        }

        public PlaneRect(Vector3 LowerLeft, Vector3 UpperLeft, Vector3 UpperRight, Vector3 LowerRight)
        {
            p00 = LowerLeft;
            p10 = UpperLeft;
            p11 = UpperRight;
            p01 = LowerRight;
        }

        public PlaneRect(Vector3 min, Vector3 max, Vector3 normal)
        {
            p00 = min;
            p11 = max;
            Vector3 cent = p00 + ((p11 - p00) / 2);
            if ((p11 - p00).normalized == normal.normalized
                || (p00 - p11).normalized == normal.normalized)
            {
                Debug.LogError("Invalid rectangle.");
                p00 = Vector3.zero;
                p01 = new Vector3(1, 0);
                p10 = new Vector3(0, 1);
                p11 = new Vector3(1, 1);
            }
            else
            {
                Vector3 dir = Vector3.Cross(p11 - p00, normal);
                //Debug.Log("Direction vector to p01 and p10 is " + dir);
                Vector3 dis = ((p11 - p00) / 2).magnitude * dir.normalized;
                //Debug.Log("Distance magnitude is " + ((p11 - p00) / 2).magnitude * dir.normalized);
                //Debug.Log("Distance is " + dis);
                p10 = cent - dis;
                p01 = cent + dis;
            }
        }
        #endregion

        #region Methods
        public Vector3 Translate(Vector3 vec)
        {
            p00 += vec;
            p01 += vec;
            p10 += vec;
            p11 += vec;

            return center;
        }

        public bool Contains(Vector3 point)
        {
            // is it to the right of p00
            Vector3 rightComp = point - p00;
            float dot = Vector3.Dot(rightComp, Right);

            // if it's to the right and comes before a U value of 1, then it's 
            // within the horizontal bounds of the rect
            if (dot < width)
            {
                Vector3 upComp = point - p00;
                dot = Vector3.Dot(upComp, Up);
                if (dot < height)
                {
                    return true;
                }
            }

            return false;
        }

        public RaycastHit Intersect(Ray ray, Vector3 origin)
        {
            //Debug.Log("p00 = " + p00);
            //Debug.Log("p01 = " + p01);
            //Debug.Log("p10 = " + p10);
            //Debug.Log("p11 = " + p11);

            float t = Vector3.Dot((position - origin), Normal) / Vector3.Dot(ray.direction.normalized, Normal);
            //Debug.Log("Normal = " + Normal);

            RaycastHit info = new RaycastHit();
            info.distance = t;
            info.normal = Normal;
            info.point = origin + ray.direction * t;

            info.barycentricCoordinate = Vector3.zero;
            return info;
        }
        #endregion

        #region Properties
        public Vector3 center
        {
            get
            {
                return p00 + ((p11 - p00) / 2);
            }
        }
        public float height
        {
            get
            {
                return (p11 - p10).magnitude;
            }
        }
        public Vector3 max
        {
            get
            {
                return p11;
            }
        }
        public Vector3 min
        {
            get
            {
                return p00;
            }
        }
        public Vector3 position
        {
            get
            {
                return center;
            }
        }
        public Vector3 size
        {
            get
            {
                return p11 - p00;
            }
        }
        public float width
        {
            get
            {
                return (p11 - p01).magnitude;
            }
        }
        public float x
        {
            get
            {
                return position.x;
            }
        }
        public float y
        {
            get
            {
                return position.y;
            }
        }
        public float z
        {
            get
            {
                return position.z;
            }
        }
        public float xMax
        {
            get
            {
                return max.x;
            }
        }
        public float xMin
        {
            get
            {
                return min.x;
            }
        }
        public float yMax
        {
            get
            {
                return max.y;
            }
        }
        public float yMin
        {
            get
            {
                return min.y;
            }
        }
        public float zMax
        {
            get
            {
                return max.z;
            }
        }
        public float zMin
        {
            get
            {
                return min.z;
            }
        }

        public Vector3 Corner00
        {
            get
            {
                return p00;
            }
        }
        public Vector3 Corner01
        {
            get
            {
                return p01;
            }
        }
        public Vector3 Corner10
        {
            get
            {
                return p10;
            }
        }
        public Vector3 Corner11
        {
            get
            {
                return p11;
            }
        }

        public Vector3 Right
        {
            get
            {
                return (p11 - p01).normalized;
            }
        }
        public Vector3 Up
        {
            get
            {
                return (p11 - p10).normalized;
            }
        }
        public Vector3 Normal
        {
            get
            {
                return Vector3.Cross(Up, Right);
            }
        }
        #endregion
    }
}