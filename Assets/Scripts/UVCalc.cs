﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class UVCalc : MonoBehaviour
    {
        public struct UVBounds
        {
            public Vector2 LowerLeft;
            public Vector2 UpperLeft;
            public Vector2 UpperRight;
            public Vector2 LowerRight;
        };

        //public double ScaleFactor = 1.0;

        public GameObject Viewer;
        public GameObject PortalCamera;

        public Vector2 LowerLeft;
        public Vector2 LowerRight;
        public Vector2 UpperLeft;
        public Vector2 UpperRight;
        
        public void Awake()
        {
            LowerLeft = Vector2.zero;
            LowerRight = Vector2.zero;
            UpperLeft = Vector2.zero;
            UpperRight = Vector2.zero;
        }

        public void Update()
        {
            if (PortalCamera != null)
            {
                Camera cam = PortalCamera.GetComponent<Camera>();
                if (cam != null)
                {
                    UVBounds bounds = ClipUV();
                    LowerLeft = bounds.LowerLeft;
                    LowerRight = bounds.LowerRight;
                    UpperLeft = bounds.UpperLeft;
                    UpperRight = bounds.UpperRight;
                }
            }
        }

        public static Vector2 UV(PlaneRect canvas, Vector3 pointOnCanvas)
        {
            Vector3 rightComp = pointOnCanvas - canvas.Corner00;
            //Debug.Log("RightComp = " + rightComp);
            //Debug.Log("pointOnCanvas = " + pointOnCanvas);
            //Debug.Log("Corner00 = " + canvas.Corner00);
            float dot = Vector3.Dot(rightComp, canvas.Right);
            float u = dot / canvas.width;
            //Debug.Log("Canvas width = " + canvas.width);
            //Debug.Log("RightDot = " + dot);
            //Debug.Log("u = " + u);
            Vector3 upComp = pointOnCanvas - canvas.Corner00;
            dot = Vector3.Dot(upComp, canvas.Up);
            float v = dot / canvas.height;

            return new Vector2(u, v);
        }

        public UVBounds ClipUV()
        {
            ClipPlaneManager clipPlane = new ClipPlaneManager(PortalCamera.GetComponent<Camera>());
            // Get the root object (portal's) rotation difference from its original rotation
            var portalScript = gameObject.transform.root.gameObject.GetComponent<PortalScript>();
            if(portalScript != null)
            {
                // ERROR WARNING: When dynamically creating a clip plane instead of updating here, you are creating a clip plane using the camera's current settings.
                // This means that you do not need to set rotate again, as that will double-rotate the object. If this clip plane were persistent, then you would have
                // to update the rotation every frame to make sure that the position of the points is still correct!

                //Quaternion deltaPortalRot = portalScript.GetRotationDiff();
                //Vector3 portalCamPos = portalScript.gameObject.GetComponentInChildren<Camera>().transform.position;
                ////clipPlane.ClipRect.RotateAroundPoint(portalCamPos, deltaPortalRot);

                //Quaternion origRot = portalScript.origRotation_m;
                //Quaternion currentRot = portalScript.gameObject.transform.rotation;
                //clipPlane.ClipRect.RotateToAroundPoint(portalCamPos, currentRot, origRot);

                // For debugging purposes only
                var debugger = GameObject.FindObjectOfType<PlaneRectDebugger>();
                debugger.cam = PortalCamera.GetComponent<Camera>();
                debugger.camParent = gameObject;
            }
            else
            {
                Debug.LogError("UVCalc could not find portal's portal script to appropriately apply rotation for UV plane calculations!");
            }

            //Vector3 origin = gameObject.transform.position;
            //GameObject portal = PortalCamera.transform.parent.gameObject;
            Vector3 origin = Viewer.transform.position;
            GameObject portal = gameObject;

            //origin = AdjustViewpoint();

            Ray r00 = Intersector.R00(portal, origin);
            Ray r01 = Intersector.R01(portal, origin);
            Ray r10 = Intersector.R10(portal, origin);
            Ray r11 = Intersector.R11(portal, origin);

            //Debug.Log("Ray for point 00 = " + r00);

            RaycastHit rh00 = clipPlane.Intersect(r00, origin);
            RaycastHit rh01 = clipPlane.Intersect(r01, origin);
            RaycastHit rh10 = clipPlane.Intersect(r10, origin);
            RaycastHit rh11 = clipPlane.Intersect(r11, origin);

            //Debug.Log("Raycasthit distance = " + rh00.distance);

            UVBounds bounds = new UVBounds()
            {
                LowerLeft = UV(clipPlane.ClipRect, rh00.point),
                UpperLeft = UV(clipPlane.ClipRect, rh01.point),
                UpperRight = UV(clipPlane.ClipRect, rh11.point),
                LowerRight = UV(clipPlane.ClipRect, rh10.point)
            };

            //Debug.Log("00 point = " + rh00.point);
            //Debug.Log("01 point = " + rh01.point);
            //Debug.Log("10 point = " + rh10.point);
            //Debug.Log("11 point = " + rh11.point);

            Debug.DrawLine(origin, rh00.point, Color.red);
            Debug.DrawLine(origin, rh01.point, Color.green);
            Debug.DrawLine(origin, rh10.point, Color.blue);
            Debug.DrawLine(origin, rh11.point, Color.white);

            Debug.DrawLine(origin, origin + clipPlane.ClipRect.Up * 5, Color.black);
            Debug.DrawLine(origin, origin + clipPlane.ClipRect.Right * 5, Color.magenta);

            return bounds;
        }

        //public Vector3 AdjustViewpoint()
        //{
        //    Vector3 origin = gameObject.transform.position;
        //    GameObject portal = PortalCamera.transform.parent.gameObject;

        //    Vector3 delta = (portal.transform.position - origin) * ((float)(ScaleFactor - 1));
        //    Vector3 adjustedOrigin = origin + delta;

        //    //GameObject DebugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    //DebugSphere.transform.Translate(adjustedOrigin);
        //    return adjustedOrigin;
        //}
    }
}