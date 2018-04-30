using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class KinectControllerManager : MonoBehaviour
    {
        public Windows.Kinect.JointType ControllerJoint = Windows.Kinect.JointType.Head;
        public BodyManager bodyManager;
        public MovementKinect movementScript;

        // Use this for initialization
        void Awake()
        {
            List<Windows.Kinect.JointType> visibleJointList = new List<Windows.Kinect.JointType>();
            visibleJointList.Add(ControllerJoint);

            bodyManager.SetVisibleJoints(visibleJointList);
        }

        private void FixedUpdate()
        {
            movementScript.ControllerJointObj = bodyManager.GetJointObject(ControllerJoint);
        }
    }
}