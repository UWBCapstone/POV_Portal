using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace ARPortal
{
    [CustomEditor(typeof(OculusPortalCreator))]
    public class OculusPortalCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            OculusPortalCreator c = (OculusPortalCreator)target;
            if(GUILayout.Button("Create portal"))
            {
                c.CreatePortal();
            }

            if(GUILayout.Button("Clean VR Camera After SteamVR Crash"))
            {
                c.CleanCameraSettingsAfterCrash();
            }
        }
    }
}
#endif