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

        private Quaternion rotation;

        #region Constructors
        public PlaneRect()
        {
            p00 = Vector3.zero;
            p01 = new Vector3(0, 1);
            p10 = new Vector3(1, 0);
            p11 = new Vector3(1, 1);

            rotation = Quaternion.identity;
        }

        public PlaneRect(PlaneRect pr)
        {
            this.p00 = pr.p00;
            this.p01 = pr.p01;
            this.p10 = pr.p10;
            this.p11 = pr.p11;

            rotation = Quaternion.identity;
        }

        public PlaneRect(Vector3 LowerLeft, Vector3 UpperLeft, Vector3 UpperRight, Vector3 LowerRight) : this()
        {
            p00 = LowerLeft;
            p10 = LowerRight;
            p11 = UpperRight;
            p01 = UpperLeft;

            rotation = Quaternion.identity;
        }

        public PlaneRect(Vector3 min, Vector3 max, Vector3 normal, bool isSquare) : this()
        {
            p00 = new Vector3(min.x, min.y, min.z);
            p11 = new Vector3(max.x, max.y, max.z);
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
                if (isSquare)
                {
                    Vector3 dir = Vector3.Cross(p11 - p00, normal);
                    //Debug.Log("Direction vector to p01 and p10 is " + dir);
                    Vector3 dis = ((p11 - p00) / 2).magnitude * dir.normalized;
                    //Debug.Log("Distance magnitude is " + ((p11 - p00) / 2).magnitude * dir.normalized);
                    //Debug.Log("Distance is " + dis);
                    p10 = cent - dis;
                    p01 = cent + dis;
                }
                else
                {
                    // Assumes it's a vertical rectangle that is horizontally level
                    Vector3 left = cent + Vector3.left;
                    float dotL = Vector3.Dot(p00 - cent, left - cent);
                    left = cent + Vector3.left * dotL;
                    p01 = p00 + (left - p00) * 2;

                    Vector3 right = cent + Vector3.right;
                    float dotR = Vector3.Dot(p11 - cent, right - cent);
                    right = cent + Vector3.right * dotR;
                    p10 = p11 + (right - p11) * 2;
                }
            }

            rotation = Quaternion.identity;
        }
        #endregion

        #region Methods
        public void CopyTo(PlaneRect pr)
        {
            pr.p00 = this.p00;
            pr.p01 = this.p01;
            pr.p11 = this.p11;
            pr.p10 = this.p10;
        }

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

        public void RotateAroundPoint(Vector3 point, Quaternion rotation)
        {
            Vector3 P00Dir = p00 - point;
            P00Dir = rotation * P00Dir;
            p00 = P00Dir + point;

            Vector3 P01Dir = p01 - point;
            P01Dir = rotation * P01Dir;
            p01 = P01Dir + point;

            Vector3 P10Dir = p10 - point;
            P10Dir = rotation * P10Dir;
            p10 = P10Dir + point;

            Vector3 P11Dir = p11 - point;
            P11Dir = rotation * P11Dir;
            p11 = P11Dir + point;
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
                Vector3 point = p00;

                Vector3 dir = point - center;
                dir = rotation * dir;
                point = dir + center;

                return point;
            }
        }
        public Vector3 Corner01
        {
            get
            {
                Vector3 point = p01;

                Vector3 dir = point - center;
                dir = rotation * dir;
                point = dir + center; 

                return point;
            }
        }
        public Vector3 Corner10
        {
            get
            {
                Vector3 point = p10;

                Vector3 dir = point - center;
                dir = rotation * dir;
                point = dir + center;

                return point;
            }
        }
        public Vector3 Corner11
        {
            get
            {
                Vector3 point = p11;

                Vector3 dir = point - center;
                dir = rotation * dir;
                point = dir + center;

                return point;
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
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