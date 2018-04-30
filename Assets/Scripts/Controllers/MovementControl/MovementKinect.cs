using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class MovementKinect : MonoBehaviour
    {
        public GameObject ControllerJointObj;
        public float scaleX = 0.08f;
        public float scaleY = 0.08f;
        public float scaleZ = 0.08f;
        public Vector3 cameraOrigin;
        public Vector3 jointOrigin = Vector3.zero;

        public void Awake()
        {
            cameraOrigin = gameObject.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if(ControllerJointObj != null)
            {
                // Set camera position to kinect's tracked joint position - expected origin
                if (jointOrigin.Equals(Vector3.zero))
                {
                    // Set the origin
                    jointOrigin = ControllerJointObj.transform.position;
                }
                
                Vector3 diff = ControllerJointObj.transform.position - jointOrigin;

                //Vector3 newPos = cameraOrigin + diff;
                Vector3 newPos = new Vector3(
                    cameraOrigin.x + diff.x * scaleX,
                    cameraOrigin.y + diff.y * scaleY,
                    cameraOrigin.z + diff.z * scaleZ
                    );

                if(newPos.z < 0)
                {
                    newPos.z = 0;
                }

                gameObject.transform.position = newPos;
            }
        }
    }
}