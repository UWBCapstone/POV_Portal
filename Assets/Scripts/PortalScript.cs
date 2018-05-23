using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    /// <summary>
    /// Handles the actions, behavior, and feed typically associated with a 
    /// single portal.
    /// </summary>
    public class PortalScript : MonoBehaviour
    {
        //public WebCamDevice webCamDevice;
        //public WebCamTexture WebCamFeed;
        public Texture2D Texture;
        public ShaderSetter shaderSetter;
        [HideInInspector]
        public Quaternion origRotation_m;

        public void Awake()
        {
            shaderSetter = gameObject.GetComponent<ShaderSetter>();
            origRotation_m = gameObject.transform.root.rotation;
        }

        public Quaternion GetRotationDiff()
        {
            Quaternion origRot = origRotation_m;
            Quaternion currentRot = gameObject.transform.rotation;
            return origRot * Quaternion.Inverse(currentRot);
        }

        public void UpdateTexture(Texture2D tex)
        {
            Texture = tex;
            shaderSetter.Texture = Texture;

            updatePortalCameraOnTextureUpdate();
        }

        private void updatePortalCameraOnTextureUpdate()
        {
            // Set the portal camera's FOV dynamically to match the webcam's
            var webcamSpecs = WebCamSpecsManager.GetSpecs(WebCamSpecsManager.WebCamDeviceToSpecsName(Texture.name));
            var horizontalFOV = webcamSpecs.HorizontalFOV;
            var verticalFOV = webcamSpecs.VerticalFOV;
            var aspect = horizontalFOV / verticalFOV;

            GameObject portalCam = null;
            int nChildren = gameObject.transform.childCount;
            for (int i = 0; i < nChildren; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child.name.Equals("PortalCam"))
                {
                    portalCam = child;
                    break;
                }
            }

            if (portalCam != null)
            {
                Camera cam = portalCam.GetComponent<Camera>();
                if (cam != null)
                {
                    WebCamSpecsManager.AssignSpecsToCameraExceptFarClipPlane(cam, webcamSpecs);
                    //cam.fieldOfView = verticalFOV;
                    //cam.aspect = aspect;
                }
            }
        }

        //public void Play()
        //{

        //}

        //public void Stop()
        //{

        //}

        public void Close()
        {
            //Stop();
            gameObject.SetActive(false);
            GameObject.Destroy(gameObject);
        }
    }
}