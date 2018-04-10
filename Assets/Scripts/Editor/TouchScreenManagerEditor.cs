using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace ARPortal
{
    [CustomEditor(typeof(TouchScreenManager))]
    public class TouchScreenManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TouchScreenManager myManager = (TouchScreenManager)target;
            if(GUILayout.Button("Simulate Tap"))
            {
                myManager.OnTap();
            }
        }
    }
}
#endif