using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class MyVideoOverlay : MonoBehaviour
    {
        public VideoOverlayManager manager;
        public Texture2D VideoTexture;
        public Texture2D WorldTexture;
        public Material WebCamFeedMaterial;

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (VideoTexture != null)
            {
                //Graphics.Blit(source, destination, WebCamFeedMaterial);
                Graphics.Blit(VideoTexture, null, WebCamFeedMaterial); // null destination = blit directly to screen
            }
        }

        public void Update()
        {
            if(manager != null)
            {
                VideoTexture = manager.VideoTexture;
                WorldTexture = manager.WorldTexture;
                WebCamFeedMaterial = manager.WebCamFeedMaterial;
            }
        }
    }
}