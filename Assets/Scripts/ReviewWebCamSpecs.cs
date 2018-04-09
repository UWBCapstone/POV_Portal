using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class ReviewWebCamSpecs : MonoBehaviour
    {
        public WebcamDeviceNames WebcamDeviceName = WebcamDeviceNames.LOGITECH_C920;

        public float HorizontalFOV;
        public float VerticalFOV;
        public float NearClippingPlane;
        public float FarClippingPlane;
        public string DeviceName;
        public int HorizontalResolution;
        public int VerticalResolution;
        
        public void Update()
        {
            WebCamSpecs specs = WebCamSpecsManager.GetSpecs(WebcamDeviceNames.NULL);

            switch (WebcamDeviceName)
            {
                case WebcamDeviceNames.ADESSO_CYBERTRACK_V10:
                    specs = WebCamSpecsManager.GetSpecs(WebcamDeviceNames.ADESSO_CYBERTRACK_V10);
                    break;
                case WebcamDeviceNames.LAPTOP_WEBCAM:
                    specs = WebCamSpecsManager.GetSpecs(WebcamDeviceNames.LAPTOP_WEBCAM);
                    break;
                case WebcamDeviceNames.LOGITECH_C920:
                    specs = WebCamSpecsManager.GetSpecs(WebcamDeviceNames.LOGITECH_C920);
                    break;
                case WebcamDeviceNames.NULL:
                    specs = WebCamSpecsManager.GetSpecs(WebcamDeviceNames.NULL);
                    break;
            }

            SetReviewedInfo(specs);
        }

        public void SetReviewedInfo(WebCamSpecs specs)
        {
            HorizontalFOV = specs.HorizontalFOV;
            VerticalFOV = specs.VerticalFOV;
            NearClippingPlane = specs.NearClippingPlane;
            FarClippingPlane = specs.FarClippingPlane;
            DeviceName = specs.DeviceName;
            HorizontalResolution = specs.HorizontalResolution;
            VerticalResolution = specs.VerticalResolution;
        }
    }
}