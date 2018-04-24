using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace ARPortal
{
    [CustomEditor(typeof(PortalCreator))]
    public class PortalCreationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PortalCreator myManager = (PortalCreator)target;
            if (GUILayout.Button("Create Portal"))
            {
                myManager.GeneratePortal();
            }
        }
    }
}
#endif