using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class MousePortalController : MonoBehaviour
    {
        public PortalCreator portalCreationManager;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Create portal on a left click
                portalCreationManager.GeneratePortal();
            }
        }
    }
}