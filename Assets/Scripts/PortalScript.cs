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

        public void Awake()
        {
            shaderSetter = gameObject.GetComponent<ShaderSetter>();
        }

        public void UpdateTexture(Texture2D tex)
        {
            Texture = tex;
            shaderSetter.Texture = Texture;
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