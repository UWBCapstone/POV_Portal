using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class WebCamFeedReviewer : MonoBehaviour
    {
        public GameObject MainCamera;
        public WebCamManager webCamManager;
        public List<GameObject> CameraGameObjects;

        public void Awake()
        {
            CameraGameObjects = new List<GameObject>();
        }

        public void Update()
        {
            if(webCamManager.NumWebCams != CameraGameObjects.Count)
            {
                RedoCameras();
            }
        }

        public void RedoCameras()
        {
            // make main camera visible during loading
            MainCamera.GetComponent<Camera>().depth = 0;

            // Clear display camera list
            for(int i = 0; i < CameraGameObjects.Count; i++)
            {
                GameObject.Destroy(CameraGameObjects[i]);
            }
            CameraGameObjects.Clear();

            for(int i = 0; i < webCamManager.NumWebCams; i++)
            {
                WebcamDeviceNames deviceName = WebCamSpecsManager.WebCamDeviceToSpecsName(webCamManager.WebCams[i]);
                GameObject displayCamObj = AddDisplayCamera(deviceName);
                AssignDisplayCameraWebCamFeed(displayCamObj, webCamManager.VideoFeeds[i]);
            }

            RedoViewport();
        }

        public GameObject AddDisplayCamera(WebcamDeviceNames deviceName)
        {
            GameObject displayCamObj = GenerateDisplayCamera(deviceName);
            CameraGameObjects.Add(displayCamObj);

            return displayCamObj;
        }

        public void AssignDisplayCameraWebCamFeed(GameObject displayCamObj, WebCamTexture feed)
        {
            // Get plane object
            GameObject plane = displayCamObj.transform.GetChild(0).gameObject;
            MeshRenderer mr = plane.GetComponent<MeshRenderer>();
            mr.material.SetTexture("_MainTex", feed);
        }

        public GameObject GenerateDisplayCamera(WebcamDeviceNames deviceName)
        {
            GameObject displayCameraObj = new GameObject();
            displayCameraObj.SetActive(false);

            WebCamSpecs specs = WebCamSpecsManager.GetSpecs(deviceName);
            Camera cam = displayCameraObj.AddComponent<Camera>();
            cam.name = specs.DeviceName;
            cam.farClipPlane = specs.FarClippingPlane;
            cam.nearClipPlane = specs.NearClippingPlane;
            cam.fieldOfView = specs.VerticalFOV;
            cam.aspect = specs.HorizontalFOV / specs.VerticalFOV;

            displayCameraObj.AddComponent<FlareLayer>();
            displayCameraObj.AddComponent<GUILayer>();

            ClipPlaneManager clipPlane = new ClipPlaneManager(cam);
            GenerateDisplayPlane(clipPlane, displayCameraObj);

            int objIndex = CameraGameObjects.Count;
            Vector3 objTranslateDir = new Vector3(1000, 0, 0) * objIndex;
            displayCameraObj.transform.Translate(objTranslateDir);

            SetActiveRecursive(displayCameraObj, true);

            return displayCameraObj;
        }

        public void GenerateDisplayPlane(ClipPlaneManager clipPlane, GameObject camObj)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.SetActive(false);
            
            plane.transform.Translate(clipPlane.ClipRect.center);
            plane.transform.Rotate(Vector3.left, 90);
            plane.transform.Rotate(Vector3.up, 180);
            plane.transform.SetParent(camObj.transform);
            plane.transform.Translate(plane.transform.forward * -0.05f);

            Material planeMat = new Material(Shader.Find("Custom/ImageShader"));
            planeMat.name = camObj.name;
            MeshRenderer mr = plane.GetComponent<MeshRenderer>();
            mr.material = planeMat;

            Bounds bounds = mr.bounds;
            Debug.Log("bounds size = " + bounds.size);
            Debug.Log("clipplane width = " + clipPlane.ClipRect.width);
            Debug.Log("clipplane height = " + clipPlane.ClipRect.height);
            float aspectRatio = camObj.GetComponent<Camera>().aspect;
            plane.transform.localScale = new Vector3(clipPlane.ClipRect.width / bounds.size.x * aspectRatio, 1, clipPlane.ClipRect.height / bounds.size.y);
        }

        public void SetActiveRecursive(GameObject camObj, bool active)
        {
            for(int i = 0; i < camObj.transform.childCount; i++)
            {
                GameObject child = camObj.transform.GetChild(i).gameObject;
                SetActiveRecursive(child, active);
            }

            camObj.SetActive(active);
        }

        public void RedoViewport()
        {
            // 1 2 or 3
            int numSplits = 0;
            int numCams = webCamManager.NumWebCams;
            while(numCams > 1)
            {
                numCams = numCams << 1;
                numSplits++;
            }

            if(numCams == 0
                && numSplits == 0)
            {
                // nothing displaying
                Camera cam = MainCamera.GetComponent<Camera>();
                Vector2 viewportPos = new Vector2(0, 0);
                Vector2 viewportSize = new Vector2(1, 1);
                cam.rect = new Rect(viewportPos, viewportSize);
                cam.depth = 0;

                foreach(GameObject otherCamObj in CameraGameObjects)
                {
                    Camera otherCam = otherCamObj.GetComponent<Camera>();
                    otherCam.depth = -1;
                }
            }
            else if(numCams == 1
                && numSplits == 0)
            {
                // 1 horizontal
                Camera cam = CameraGameObjects[0].GetComponent<Camera>();
                Vector2 viewportPos = new Vector2(0, 0);
                Vector2 viewportSize = new Vector2(1, 1);
                cam.rect = new Rect(viewportPos, viewportSize);
                cam.depth = 0;

                Camera otherCam = MainCamera.GetComponent<Camera>();
                otherCam.depth = -1;
            }
            else if(numCams == 1
                && numSplits == 1)
            {
                // 3 horizontal
                for(int i = 0; i < CameraGameObjects.Count; i++)
                {
                    Camera cam = CameraGameObjects[i].GetComponent<Camera>();
                    Vector2 viewportPos = new Vector2(i * (1 / 3.0f), 0);
                    Vector2 viewportSize = new Vector2((1 / 3.0f), 1);
                    cam.rect = new Rect(viewportPos, viewportSize);
                    cam.depth = 0;
                }

                Camera otherCam = MainCamera.GetComponent<Camera>();
                otherCam.depth = -1;
            }
            else if(numCams == 1
                && numSplits > 2)
            {
                int camObjIndex = 0;

                // 3 horizontal first split
                for(int i = 0; i < 3; i++)
                {
                    Vector2 viewportPos = new Vector2(i * (1 / 3.0f), 0);
                    Vector2 viewportSize = new Vector2((1 / 3.0f), 1);

                    RecursiveViewportSplit(viewportPos, viewportSize, true, numSplits-1, ref camObjIndex);
                }

                Camera otherCam = MainCamera.GetComponent<Camera>();
                otherCam.depth = -1;
            }
            else
            {
                // normal binary split
                int camObjIndex = 0;
                Vector2 viewportPos = new Vector2(0, 0);
                Vector2 viewportSize = new Vector2(1, 1);
                RecursiveViewportSplit(viewportPos, viewportSize, false, numSplits - 1, ref camObjIndex);

                Camera otherCam = MainCamera.GetComponent<Camera>();
                otherCam.depth = -1;
            }

        }

        public void RecursiveViewportSplit(Vector2 viewportPos, Vector2 viewportSize, bool splitByHorizontalLine, int numSplits, ref int camObjIndex)
        {
            if(numSplits == 0)
            {
                // apply camera logic
                Camera cam = CameraGameObjects[camObjIndex].GetComponent<Camera>();
                cam.rect = new Rect(viewportPos, viewportSize);
                cam.depth = 0;

                camObjIndex++;

                return;
            }
            else if(numSplits < 0)
            {
                return;
            }
            else
            {
                Vector2 newViewportPos1 = viewportPos;
                Vector2 newViewportSize1;
                if (splitByHorizontalLine)
                {
                    newViewportSize1 = new Vector2(viewportSize.x, viewportSize.y / 2.0f);
                }
                else
                {
                    newViewportSize1 = new Vector2(viewportSize.x / 2.0f, viewportSize.y);
                }

                RecursiveViewportSplit(newViewportPos1, newViewportSize1, !splitByHorizontalLine, numSplits - 1, ref camObjIndex);

                Vector2 newViewportPos2;
                Vector2 newViewportSize2 = newViewportSize1;
                if (splitByHorizontalLine)
                {
                    newViewportPos2 = new Vector2(viewportPos.x, viewportPos.y + newViewportSize1.y);
                }
                else
                {
                    newViewportPos2 = new Vector2(viewportPos.x + newViewportSize1.x, viewportPos.y);
                }

                RecursiveViewportSplit(newViewportPos2, newViewportSize2, !splitByHorizontalLine, numSplits - 1, ref camObjIndex);
            }

        }
    }
}