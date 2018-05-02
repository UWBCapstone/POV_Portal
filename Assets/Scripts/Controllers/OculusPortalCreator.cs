using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class OculusPortalCreator : MonoBehaviour
    {
        public GameObject ControllerObject;
        public PortalCreator portalCreator;

        public void CreatePortal()
        {
            RaycastHit hitInfo = RaycastToEnvironment();
            Vector3 portalPos = hitInfo.point;// getthe hitinfo fro the collision
            //portalCreator.StartPosition = portalPos;
            GameObject portal = portalCreator.GeneratePortal(portalPos);
            AdjustPortalRotation(portal, hitInfo);
        }

        public RaycastHit RaycastToEnvironment()
        {
            RaycastHit hitInfo;
            Ray ray = new Ray(ControllerObject.transform.position, ControllerObject.transform.forward);
            Physics.Raycast(ray, out hitInfo);

            return hitInfo;
        }

        public void AdjustPortalRotation(GameObject portal, RaycastHit hitInfo)
        {
            GameObject collidedObject = hitInfo.collider.gameObject;
            Vector3 collidedForward = collidedObject.transform.forward;

            portal.transform.forward = collidedForward;
        }
    }
}