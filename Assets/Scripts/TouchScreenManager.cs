using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class TouchScreenManager : MonoBehaviour
    {
        public PortalCreator portalCreator;
        
        public void Awake()
        {
            //portalCreator = new PortalCreator();

            //
            //GameObject portal = portalCreator.GeneratePortal();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchSupported)
            {
                bool touched = false;

                if(Input.touches.Length > 0)
                {
                    foreach(Touch touch in Input.touches)
                    {
                        if(touch.phase == TouchPhase.Began)
                        {
                            touched = true;
                            break;
                        }
                    }
                }

                if (touched)
                {
                    //GameObject portal = portalCreator.GeneratePortal();
                    OnTap();
                }
            }
        }

        public void OnTap()
        {
            GameObject portal = portalCreator.GeneratePortal();
        }
    }
}