using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public static class FeedTextureTranslator
    {
        public static List<Texture2D> GetTexturesFrom(List<WebCamTexture> feedList)
        {
            List<Texture2D> texList = new List<Texture2D>();
            for(int i = 0; i < feedList.Count; i++)
            {
                texList.Add(GetTextureFrom(feedList[i]));
            }

            return texList;
        }

        public static Texture2D GetTextureFrom(WebCamTexture feed)
        {
            if (feed != null)
            {
                Texture2D tex = new Texture2D(feed.width, feed.height, TextureFormat.RGBA32, false);
                tex.SetPixels(feed.GetPixels());
                tex.Apply();

                return tex;
            }

            return null;
        }
    }
}