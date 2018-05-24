using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace ARPortal
{
    [CustomEditor(typeof(PlaneRectDebugger))]
    public class PlaneRectDebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PlaneRectDebugger debugger = (PlaneRectDebugger)target;
            if(GUILayout.Button("Update Clip Plane Rotation"))
            {
                debugger.UpdateClipPlaneRotation();
            }
        }

    }
}