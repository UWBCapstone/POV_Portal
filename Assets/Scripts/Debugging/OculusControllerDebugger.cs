using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class OculusControllerDebugger : MonoBehaviour
    {
        // Left Touch Controller
        public bool LeftThumbstickIn = false; // Button.PrimaryThumbstick
        public Vector2 LeftThumbstickValue = Vector2.zero; // Axis2D.PrimaryThumbstick
        public bool X = false; // Button.Three
        public bool Y = false; // Button.Four
        public bool LeftButtonStart = false; // Button.Start
        public float LeftIndexTrigger = 0.0f; // Axis1D.PrimaryIndexTrigger
        public float LeftHandTrigger = 0.0f; // Axis1D.PrimaryHandTrigger

        // Right Touch Controller
        public bool RightThumbstickIn = false; // Button.SecondaryThumbstick
        public Vector2 RightThumbstickValue = Vector2.zero; // Axis2D.SecondaryThumbstick
        public bool A = false; // Button.One
        public bool B = false; // Button.Four
        public bool RightButtonReserved = false; // Reserved
        public float RightIndexTrigger = 0.0f; // Axis1D.SecondaryIndexTrigger
        public float RightHandTrigger = 0.0f; // Axis1D.SecondaryHandTrigger

        public void Update()
        {
            CheckLeftController();
            CheckRightController();
        }

        public void DisplayMsgFor(string item)
        {
            Debug.Log(item + " was pressed!");
        }

        public void CheckLeftController()
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                LeftThumbstickIn = true;
            }
            else
            {
                LeftThumbstickIn = false;
            }

            LeftThumbstickValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            if (OVRInput.Get(OVRInput.Button.Three))
            {
                X = true;
            }
            else
            {
                X = false;
            }

            if (OVRInput.Get(OVRInput.Button.Four))
            {
                Y = true;
            }
            else
            {
                Y = false;
            }

            if (OVRInput.Get(OVRInput.Button.Start))
            {
                LeftButtonStart = true;
            }
            else
            {
                LeftButtonStart = false;
            }

            LeftIndexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            LeftHandTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        }

        public void CheckRightController()
        {
            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                RightThumbstickIn = true;
            }
            else
            {
                RightThumbstickIn = false;
            }

            RightThumbstickValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            if (OVRInput.Get(OVRInput.Button.One))
            {
                A = true;
            }
            else
            {
                A = false;
            }

            if (OVRInput.Get(OVRInput.Button.Two))
            {
                B = true;
            }
            else
            {
                B = false;
            }

            RightIndexTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            RightHandTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        }
    }
}