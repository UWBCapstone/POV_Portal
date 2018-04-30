using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class VideoOverlayUVManager : MonoBehaviour
    {
        public float ImageDistance = 3f;
        public float SimulatedUserIsCloserBy = 1f; // Assumes the user will be 1ft closer to the imaginary plane for the background image when at world origin
        public float PortalFOV = 60f;
        public GameObject videoOverlayManager;
        public string MainCameraName = "MainCamera";
        
        public Vector2 LowerLeft;
        public Vector2 UpperLeft;
        public Vector2 UpperRight;
        public Vector2 LowerRight;

        public Camera UserCam;
        private ClipPlaneManager overallClipPlane;
        private ClipPlaneManager backgroundClipPlane;

        public void Awake()
        {
            Vector3 toImage = new Vector3(0, 0, 1);
            GetMainCamera().transform.Translate(toImage * SimulatedUserIsCloserBy);

            Init();
        }

        public void Update()
        {
            updateUserCamera();
            updateBackgroundClipPlane();
            UpdateOverlayMaterial();
        }

        public void Init()
        {
            GameObject UVCamera = generateCameraObject();
            GameObject parent = videoOverlayManager;
            UVCamera.transform.SetParent(parent.transform);

            // Attach custom scripts
            //parent.AddComponent<BoundCalc>();
            //ShaderSetter shaderSetter = parent.AddComponent<ShaderSetter>();

            UserCam = generateUserCamera();
            overallClipPlane = generateOverallClipPlane(UVCamera.GetComponent<Camera>());
            backgroundClipPlane = generateBackgroundClipPlane(UserCam);

            //parent.AddComponent<PortalScript>();
            //UVCalc uvCalc = parent.AddComponent<UVCalc>();
            //uvCalc.Viewer = GetMainCamera();
            //uvCalc.PortalCamera = UVCamera;
            
            // Reactivate the portal to make it visible now that everything
            // has loaded
            //parent.SetActive(true);
        }

        public ClipPlaneManager generateOverallClipPlane(Camera UVCamera)
        {
            ClipPlaneManager overallClipPlane = new ClipPlaneManager(UVCamera); // this clip plane manages the overall camera and gets the overall imaginary uv points for comparison
            return overallClipPlane;
        }

        public Camera generateUserCamera()
        {
            Camera MainCam = GetMainCameraComponent();
            if (MainCam != null)
            {
                Camera userCam = generateUserCamera(MainCam);
                return userCam;
            }

            return null;
        }

        public ClipPlaneManager generateBackgroundClipPlane(Camera UserCam)
        {
            if (UserCam != null)
            {
                ClipPlaneManager bcp = new ClipPlaneManager(UserCam);
                return bcp;
            }
            else
            {
                Debug.LogError("User camera not found!");
            }

            return null;
        }

        public Camera generateUserCamera(Camera MainCam)
        {
            //Camera userCam = new Camera();
            Camera userCam = gameObject.AddComponent<Camera>();
            userCam.fieldOfView = MainCam.fieldOfView;
            userCam.aspect = MainCam.aspect;
            userCam.depth = -1;
            userCam.transform.position = MainCam.transform.position;
            userCam.transform.rotation = MainCam.transform.rotation;
            userCam.transform.localScale = MainCam.transform.localScale;
            userCam.farClipPlane = calculateUserCamFarClipPlane();
            userCam.nearClipPlane = MainCam.nearClipPlane;
            
            return userCam;
        }

        // Assumes you're facing the positive z direction and not rotating at all
        // If assumption fails, you'll have to have bgPos be the new center of the far clip plane
        private float calculateUserCamFarClipPlane()
        {
            Vector3 bgPos = new Vector3(0, 0, ImageDistance);
            if (UserCam == null)
            {
                return ImageDistance - SimulatedUserIsCloserBy;
            }
            else
            {
                return (bgPos - UserCam.transform.position).z;
            }
        }

        private void updateUserCamera()
        {
            if(UserCam != null)
            {
                Camera MainCam = GetMainCameraComponent();
                if (MainCam != null)
                {
                    UserCam.fieldOfView = MainCam.fieldOfView;
                    UserCam.farClipPlane = calculateUserCamFarClipPlane();
                    UserCam.nearClipPlane = MainCam.nearClipPlane;

                    UserCam.transform.position = MainCam.transform.position;
                    UserCam.transform.rotation = MainCam.transform.rotation;
                    UserCam.transform.localScale = MainCam.transform.localScale;
                }
            }
        }

        private void updateBackgroundClipPlane()
        {
            if(backgroundClipPlane != null
                && UserCam != null)
            {
                backgroundClipPlane.UpdateInfo(UserCam);
            }
        }

        public GameObject generateCameraObject()
        {
            GameObject uvCamera = new GameObject();
            uvCamera.name = "UVCamera";
            uvCamera.AddComponent<Camera>();

            setUVCameraSettings(uvCamera);

            // Disable audio listener for additional cameras to avoid glitches 
            // with Unity
            AudioListener audioListener = uvCamera.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }

            return uvCamera;
        }

        private void setUVCameraSettings(GameObject uvCamera)
        {
            Camera uvCam = uvCamera.GetComponent<Camera>();
            if (uvCam == null)
            {
                Debug.LogError("Portal camera object does not contain camera component.");
            }
            else
            {
                uvCam.nearClipPlane = 0.3f;
                //portalCam.farClipPlane = 1000f;
                uvCam.farClipPlane = ImageDistance;
                uvCam.fieldOfView = PortalFOV;
                uvCam.depth = -1;
            }
        }

        public GameObject GetMainCamera()
        {
            return GameObject.Find(MainCameraName);
        }

        public Camera GetMainCameraComponent()
        {
            GameObject MainCamera = GetMainCamera();
            if(MainCamera != null)
            {
                return MainCamera.GetComponent<Camera>();
            }

            return null;
        }

        public Material GetOverlayMaterial()
        {
            VideoOverlayManager vom = videoOverlayManager.GetComponent<VideoOverlayManager>();
            if(vom != null)
            {
                return vom.WebCamFeedMaterial;
            }

            return null;
        }

        public void UpdateOverlayMaterial()
        {
            Material mat = GetOverlayMaterial();

            if (mat != null)
            {
                Vector2 LowerLeft = UVCalc.UV(overallClipPlane.ClipRect, backgroundClipPlane.ClipRect.Corner00);
                Vector2 UpperLeft = UVCalc.UV(overallClipPlane.ClipRect, backgroundClipPlane.ClipRect.Corner01);
                //Vector2 UpperRight = UVCalc.UV(overallClipPlane.ClipRect, backgroundClipPlane.ClipRect.Corner11);
                Vector2 LowerRight = UVCalc.UV(overallClipPlane.ClipRect, backgroundClipPlane.ClipRect.Corner10);

                this.LowerLeft = LowerLeft;
                this.UpperLeft = UpperLeft;
                this.LowerRight = LowerRight;

                mat.SetVector("_BGUVLL", LowerLeft);
                mat.SetVector("_BGUVUL", UpperLeft);
                mat.SetVector("_BGUVLR", LowerRight);
            }
        }
    }
}