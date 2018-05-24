using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class MousePortalController : MonoBehaviour
    {
        public bool RaycastToSpawn = true;
        public PortalCreator portalCreationManager;

        // Update is called once per frame
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!RaycastToSpawn)
                {
                    // Create portal on a left click
                    portalCreationManager.GeneratePortal();
                }
                else
                {
                    RaycastHit hitInfo;

                    if(hitInfoFromWall(Input.mousePosition, out hitInfo))
                    {
                        Vector3 hitNormal = hitInfo.normal;
                        Vector3 hitPoint = hitInfo.point;

                        var portal = portalCreationManager.GeneratePortal(hitPoint);
                        setForward(portal, hitNormal);
                    }
                    else
                    {
                        portalCreationManager.GeneratePortal();
                    }
                }
            }
        }

        private bool hitInfoFromWall(Vector3 mousePosScreenSpace, out RaycastHit hitInfo)
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosScreenSpace.x, mousePosScreenSpace.y, Camera.main.nearClipPlane));
            Ray r = new Ray(Camera.main.transform.position, (worldMousePos - Camera.main.transform.position));
            
            if(Physics.Raycast(r, out hitInfo))
            {
                return true;
            }

            return false;
        }

        private void setForward(GameObject portal, Vector3 desiredForward)
        {
            portal.transform.forward = -desiredForward;
        }
    }
}