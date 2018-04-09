using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class Intersector : MonoBehaviour
    {
        public static Ray R00(GameObject go, Vector3 viewpoint)
        {
            Vector3 dir = BoundCalc.Corner00(go) - viewpoint;
            Ray R00 = new Ray(viewpoint, dir);
            return R00;
        }

        public static Ray R01(GameObject go, Vector3 viewpoint)
        {
            Vector3 dir = BoundCalc.Corner01(go) - viewpoint;
            Ray R01 = new Ray(viewpoint, dir);
            return R01;
        }

        public static Ray R10(GameObject go, Vector3 viewpoint)
        {
            Vector3 dir = BoundCalc.Corner10(go) - viewpoint;
            Ray R10 = new Ray(viewpoint, dir);
            return R10;
        }

        public static Ray R11(GameObject go, Vector3 viewpoint)
        {
            Vector3 dir = BoundCalc.Corner11(go) - viewpoint;
            Ray R11 = new Ray(viewpoint, dir);
            return R11;
        }
    }
}