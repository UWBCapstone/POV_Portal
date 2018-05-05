using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    /// <summary>
    /// Unity seems to have a backend for their Resources that tracks and holds 
    /// onto Textures and GameObjects created, without releasing those Textures 
    /// when they're no longer used. This means that there will always be a 
    /// reference to generated Texture2Ds. This class just calls 
    /// Resources.UnloadUnusedAssets repeatedly to clear the project and prevent 
    /// the project from stacking up too high of a memory cost.
    /// </summary>
    public class AssetCleaner : MonoBehaviour
    {
        public bool Clean = true;

        // Update is called once per frame
        void Update()
        {
            if (Clean)
            {
                Resources.UnloadUnusedAssets();
            }
        }
    }
}