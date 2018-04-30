using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class MovementMouse : MonoBehaviour
    {
        public float xScale = 1f;
        public float zScale = 0f;
        public float posX = 0f;
        public float posZ = 0f;
        
        // Update is called once per frame
        void Update()
        {
            posX += Input.GetAxis("Mouse X") * xScale;
            posZ += Input.GetAxis("Mouse Y") * zScale;

            gameObject.transform.position = new Vector3(posX, 0, posZ);
        }
    }
}