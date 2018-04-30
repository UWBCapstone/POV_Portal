using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public enum WebcamDeviceNames
    {
        NULL,
        LOGITECH_C920,
        ADESSO_CYBERTRACK_V10,
        LAPTOP_WEBCAM,
        KINECT_2
    };

    public struct WebCamSpecs
    {
        public float HorizontalFOV;
        public float VerticalFOV;
        public float NearClippingPlane;
        public float FarClippingPlane;
        public WebcamDeviceNames WebcamDeviceName;
        public string DeviceName;
        public int HorizontalResolution;
        public int VerticalResolution;

        public WebCamSpecs(
            float horizontalFOV, 
            float verticalFOV, 
            float nearClippingPlane, 
            float farClippingPlane, 
            WebcamDeviceNames webcamDeviceName,
            string deviceName,
            int horizontalResolution,
            int verticalResolution
            )
        {
            HorizontalFOV = horizontalFOV;
            VerticalFOV = verticalFOV;
            NearClippingPlane = nearClippingPlane;
            FarClippingPlane = farClippingPlane;
            WebcamDeviceName = webcamDeviceName;
            DeviceName = deviceName;
            HorizontalResolution = horizontalResolution;
            VerticalResolution = verticalResolution;
        }
    }
    
    public static class WebCamSpecsManager
    {
        public static float DefaultNearClippingPlane = 0.3f;
        public static float DefaultFarClippingPlane = 3.0f;

        static WebCamSpecsManager()
        {

        }

        public static WebCamSpecs GetSpecs(WebcamDeviceNames deviceName)
        {
            WebCamSpecs camSpecs = new WebCamSpecs();

            float HorizontalFOV;
            float VerticalFOV;
            float NearClippingPlane;
            float FarClippingPlane;
            WebcamDeviceNames WebcamDeviceName;
            string DeviceName;
            int HorizontalResolution;
            int VerticalResolution;

            switch (deviceName)
            {
                // Dimensions
                // W: 10"W paper 10" away from camera
                // H: 8.5"H paper 11.125" away from camera
                case WebcamDeviceNames.LOGITECH_C920:
                    // http://therandomlab.blogspot.com/2013/03/logitech-c920-and-c910-fields-of-view.html
                    //HorizontalFOV = 70.42f;
                    //VerticalFOV = 43.30f;
                    HorizontalFOV = 26.565f;
                    VerticalFOV = 20.908f;
                    NearClippingPlane = DefaultNearClippingPlane;
                    FarClippingPlane = DefaultFarClippingPlane;
                    WebcamDeviceName = WebcamDeviceNames.LOGITECH_C920;
                    DeviceName = WebcamDeviceName.ToString();
                    HorizontalResolution = 640;
                    VerticalResolution = 480;

                    camSpecs = new WebCamSpecs(
                        HorizontalFOV,
                        VerticalFOV,
                        NearClippingPlane,
                        FarClippingPlane,
                        WebcamDeviceName,
                        DeviceName,
                        HorizontalResolution,
                        VerticalResolution);

                    break;
                // Dimensions
                // W: 10"W paper 12" away from camera
                // H: 8.5"H paper 15" away from camera
                case WebcamDeviceNames.ADESSO_CYBERTRACK_V10:
                    //HorizontalFOV = 25.0f;
                    //VerticalFOV = 25.0f;
                    HorizontalFOV = 22.620f;
                    VerticalFOV = 15.819f;
                    NearClippingPlane = DefaultNearClippingPlane;
                    FarClippingPlane = DefaultFarClippingPlane;
                    WebcamDeviceName = WebcamDeviceNames.ADESSO_CYBERTRACK_V10;
                    DeviceName = WebcamDeviceName.ToString();
                    HorizontalResolution = 320;
                    VerticalResolution = 240;

                    camSpecs = new WebCamSpecs(
                        HorizontalFOV,
                        VerticalFOV,
                        NearClippingPlane,
                        FarClippingPlane,
                        WebcamDeviceName,
                        DeviceName,
                        HorizontalResolution,
                        VerticalResolution);

                    break;
                // Dimensions
                // W: 10"W paper 11" away from camera
                // H: 8.5"H paper 11.75" away from camera
                case WebcamDeviceNames.LAPTOP_WEBCAM:
                    //HorizontalFOV = 75.0f;
                    //VerticalFOV = 56.25f;
                    HorizontalFOV = 24.4f;
                    VerticalFOV = 19.9f;
                    NearClippingPlane = DefaultNearClippingPlane;
                    FarClippingPlane = DefaultFarClippingPlane;
                    WebcamDeviceName = WebcamDeviceNames.LAPTOP_WEBCAM;
                    DeviceName = WebcamDeviceName.ToString();
                    HorizontalResolution = 640;
                    VerticalResolution = 480;

                    camSpecs = new WebCamSpecs(
                        HorizontalFOV,
                        VerticalFOV,
                        NearClippingPlane,
                        FarClippingPlane,
                        WebcamDeviceName,
                        DeviceName,
                        HorizontalResolution,
                        VerticalResolution);

                    break;
                case WebcamDeviceNames.KINECT_2:
                    HorizontalFOV = 57.0f;
                    VerticalFOV = 43.0f;
                    NearClippingPlane = DefaultNearClippingPlane;
                    FarClippingPlane = DefaultFarClippingPlane;
                    WebcamDeviceName = WebcamDeviceNames.KINECT_2;
                    DeviceName = WebcamDeviceName.ToString();
                    HorizontalResolution = 1920;
                    VerticalResolution = 1080;

                    camSpecs = new WebCamSpecs(
                        HorizontalFOV,
                        VerticalFOV,
                        NearClippingPlane,
                        FarClippingPlane,
                        WebcamDeviceName,
                        DeviceName,
                        HorizontalResolution,
                        VerticalResolution);

                    break;
                case WebcamDeviceNames.NULL:
                    HorizontalFOV = 0.0f;
                    VerticalFOV = 0.0f;
                    NearClippingPlane = 0.0f;
                    FarClippingPlane = 0.0f;
                    WebcamDeviceName = WebcamDeviceNames.NULL;
                    DeviceName = WebcamDeviceName.ToString();
                    HorizontalResolution = 0;
                    VerticalResolution = 0;

                    camSpecs = new WebCamSpecs(
                        HorizontalFOV,
                        VerticalFOV,
                        NearClippingPlane,
                        FarClippingPlane,
                        WebcamDeviceName,
                        DeviceName,
                        HorizontalResolution,
                        VerticalResolution);
                    break;
                default:
                    camSpecs = new WebCamSpecs();
                    Debug.LogError("Webcam Device name not recognized. To add webcam device, please modify WebCamSpecs class.");
                    break;
            }

            return camSpecs;
        }

        public static WebcamDeviceNames WebCamDeviceToSpecsName(WebCamDevice webcam)
        {
            string deviceName = webcam.name;

            if (deviceName.ToLower().Contains("HD Pro Webcam C920".ToLower()))
            {
                return WebcamDeviceNames.LOGITECH_C920;
            }
            else if (deviceName.ToLower().Contains("Adesso".ToLower())
                || deviceName.ToLower().Contains("USB 2.0".ToLower())
                || deviceName.ToLower().Contains("USB2.0".ToLower()))
            {
                return WebcamDeviceNames.ADESSO_CYBERTRACK_V10;
            }
            else if(deviceName.ToLower().Contains("Integrated Webcam".ToLower()))
            {
                return WebcamDeviceNames.LAPTOP_WEBCAM;
            }
            else if(deviceName.ToLower().Contains("Kinect V2 Video Sensor".ToLower()))
            {
                return WebcamDeviceNames.KINECT_2;
            }
            else
            {
                Debug.LogError("Unfamiliar webcam device encountered: " + deviceName);
                return WebcamDeviceNames.NULL;
            }
        }

        public static void AssignSpecsToCamera(Camera cam, WebCamSpecs specs)
        {

            cam.fieldOfView = specs.VerticalFOV;
            cam.aspect = (float)((double)specs.HorizontalFOV / (double)specs.VerticalFOV);
            cam.near = specs.NearClippingPlane;
            cam.far = specs.FarClippingPlane;
        }
    }
}