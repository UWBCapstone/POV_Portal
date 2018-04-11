using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class PortalManager : MonoBehaviour
    {
        public const int MaxPortals = 3;
        public GameObject MainCamera;
        public List<GameObject> PortalList;
        public List<Texture2D> TextureList;

        private int cyclicIndex = 0;

        public void Awake()
        {
            PortalList = new List<GameObject>();
            TextureList = new List<Texture2D>();
        }

        public void Update()
        {
            for(int i = 0; i < PortalList.Count; i++)
            {
                if(TextureList.Count >= i)
                {
                    PortalScript ps = PortalList[i].GetComponent<PortalScript>();
                    ps.UpdateTexture(TextureList[i]);
                }
            }
        }

        /// <summary>
        /// Overwrite existing textures with textures of the same name. 
        /// Otherwise, add them to the list.
        /// </summary>
        /// <param name="texList"></param>
        public void UpdateTextures(List<Texture2D> texList)
        {
            if (texList.Count > 0)
            {
                Debug.Log("UpdateTextures called by NetworkManager");
            }

            bool texFound = false;

            for(int i = 0; i < texList.Count; i++)
            {
                Texture2D tex = texList[i];
                for(int j = 0; j < TextureList.Count; j++)
                {
                    Texture2D storedTex = TextureList[j];
                    if (tex.name.Equals(storedTex.name))
                    {
                        TextureList[j] = tex;
                        texFound = true;
                        break;
                    }
                }
                //if (!texFound)
                if(true)
                {
                    TextureList.Add(tex);
                    Debug.Log("Adding texture to texture list in portalManager");
                }
            }
        }

        //public bool SetAsActivePortal(GameObject portal)
        //{
        //    if (portal != null)
        //    {
        //        GameObject mainCam = MainCamera;
        //        UVCalc camTracker = mainCam.GetComponent<UVCalc>();
        //        if (camTracker == null)
        //        {
        //            // If the camera hasn't initialized as a portal camera, make 
        //            // it a portal camera
        //            camTracker = mainCam.AddComponent<UVCalc>();
        //        }

        //        GameObject portalCamObj = portal.transform.GetChild(0).gameObject;
        //        camTracker.PortalCamera = portalCamObj;

        //        ShaderSetter shaderSetter = portal.GetComponent<ShaderSetter>();
        //        if (shaderSetter != null)
        //        {
        //            shaderSetter.Viewer = mainCam;
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public void RegisterPortal(GameObject portal)
        {
            if (PortalList.Count >= MaxPortals)
            {
                GameObject portalToClose = PortalList[cyclicIndex];

                PortalList[cyclicIndex] = portal;
                cyclicIndex++;
                cyclicIndex %= MaxPortals;

                ClosePortal(portal);
            }
            else
            {
                PortalList.Add(portal);
            }
        }

        public void ClosePortal(GameObject portal)
        {
            PortalScript rts = portal.GetComponent<PortalScript>();
            if (rts != null)
            {
                rts.Close();
            }
        }

        public void ClosePortals()
        {
            while(PortalList.Count > 0)
            {
                GameObject portal = PortalList[0];
                PortalList.Remove(portal);
                ClosePortal(portal);
            }
        }
        
        /// <summary>
        /// [deprecated] Not currently used.
        /// </summary>
        /// <param name="portalCam"></param>
        private void disableCamerasOtherThan(Camera portalCam)
        {
            Camera[] allCams = Camera.allCameras;
            for (int i = 0; i < allCams.Length; i++)
            {
                if (allCams[i] != portalCam)
                {
                    allCams[i].enabled = false;
                }
            }
        }

        public int NumPortals
        {
            get
            {
                return PortalList.Count;
            }
        }

    }
}