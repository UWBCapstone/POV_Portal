using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class VideoOverlayManager : MonoBehaviour
    {
        public Camera MainCamera;
        public Camera RenderCamera;
        public WebCamManager webCamManager;
        public WebcamDeviceNames BackgroundCamera;
        public WebCamTexture camFeed;
        public Texture2D VideoTexture;
        public Texture2D WorldTexture;
        public Material WebCamFeedMaterial;

        private Rect r;

        public void Awake()
        {
            RenderTexture rt = new RenderTexture(RenderCamera.pixelWidth, RenderCamera.pixelHeight, 0);
            RenderCamera.targetTexture = rt;
            RenderCamera.Render();
            WorldTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            
            r = new Rect(0, 0, RenderCamera.pixelWidth, RenderCamera.pixelHeight);
        }

        public void Update()
        {
            if (camFeed == null)
            {
                if (webCamManager.NumWebCams > 0)
                {
                    //camFeed = webCamManager.VideoFeeds[webCamManager.NumVideoFeeds-1];
                    camFeed = webCamManager.GetTextureFor(BackgroundCamera);
                }
            }
            VideoTexture = FeedTextureTranslator.GetTextureFrom(camFeed);
            WebCamFeedMaterial.SetTexture("_MainTex", VideoTexture);
            
            RenderTexture.active = RenderCamera.targetTexture;
            //WorldTexture.ReadPixels(new Rect(0, 0, RenderCamera.pixelWidth, RenderCamera.pixelHeight), 0, 0);
            WorldTexture.ReadPixels(r, 0, 0);
            WorldTexture.Apply();
            RenderTexture.active = null;

            WebCamFeedMaterial.SetTexture("_WorldTex", WorldTexture);
        }
    }
}