using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class PlaneRectDebugger : MonoBehaviour
    {
        public bool Debug = false;
        public Color lineColor = Color.red;

        public GameObject LowerLeftSphere;
        public GameObject UpperLeftSphere;
        public GameObject UpperRightSphere;
        public GameObject LowerRightSphere;
        public Camera cam;
        public GameObject camParent;
        public ClipPlaneManager clipPlane;

        private Camera cachedCam;
        private GameObject cachedParent;
        private Quaternion oldRotation;

        public void Awake()
        {
            cachedCam = cam;

            clipPlane = new ClipPlaneManager(cam);
            //clipPlane.ClipRect.RotateAroundPoint(cam.transform.position, cam.transform.rotation);
            oldRotation = Quaternion.identity;
        }

        public void Update()
        {
            UpdateCameraParent();
            UpdateCamera();

            if (Debug)
            {
                // Update the position of corner spheres
                if (clipPlane != null)
                {
                    LowerLeftSphere.transform.position = clipPlane.ClipRect.Corner00;
                    UpperLeftSphere.transform.position = clipPlane.ClipRect.Corner01;
                    UpperRightSphere.transform.position = clipPlane.ClipRect.Corner11;
                    LowerRightSphere.transform.position = clipPlane.ClipRect.Corner10;

                    LowerLeftSphere.SetActive(true);
                    UpperLeftSphere.SetActive(true);
                    UpperRightSphere.SetActive(true);
                    LowerRightSphere.SetActive(true);

#if UNITY_EDITOR
                    Vector3 start = LowerLeftSphere.transform.position;
                    Vector3 end = UpperLeftSphere.transform.position;

                    UnityEngine.Debug.DrawLine(start, end, lineColor);
                    start = end;
                    end = UpperRightSphere.transform.position;
                    UnityEngine.Debug.DrawLine(start, end, lineColor);
                    start = end;
                    end = LowerRightSphere.transform.position;
                    UnityEngine.Debug.DrawLine(start, end, lineColor);
                    start = end;
                    end = LowerLeftSphere.transform.position;
                    UnityEngine.Debug.DrawLine(start, end, lineColor);
#endif
                }
            }
            else
            {
                LowerLeftSphere.SetActive(false);
                UpperLeftSphere.SetActive(false);
                UpperRightSphere.SetActive(false);
                LowerRightSphere.SetActive(false);
            }
        }

        public void UpdateCameraParent()
        {
            if(camParent != cachedParent)
            {
                Quaternion newRotation = camParent.transform.rotation;
                Vector3 point = camParent.transform.position;

                Quaternion temp = Quaternion.identity;

                clipPlane.ClipRect.RotateToAroundPoint(point, newRotation, oldRotation);
                oldRotation = newRotation;

                cachedParent = camParent;
            }
        }

        public void UpdateCamera()
        {
            if(cachedCam != cam)
            {
                clipPlane = new ClipPlaneManager(cam);
                cachedCam = cam;
                oldRotation = Quaternion.identity;
                UpdateClipPlaneRotation();
            }
        }

        public void UpdateClipPlaneRotation()
        {
            if (camParent != null)
            {
                Quaternion newRotation = camParent.transform.rotation;
                Vector3 point = camParent.transform.position;
                clipPlane.ClipRect.RotateToAroundPoint(point, newRotation, oldRotation);
                oldRotation = newRotation;
            }
            else
            {
                //clipPlane.ClipRect.RotateAroundPoint(cam.transform.position, cam.transform.rotation);
                Quaternion newRotation = cam.transform.rotation;
                clipPlane.ClipRect.RotateToAroundPoint(cam.transform.position, newRotation, oldRotation);
                oldRotation = newRotation;
            }
        }
    }
}