using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tango;

// PortalShader - shader for portal generated dynamically

namespace ARPortal
{
    public struct PortalTransform
    {
        public Vector3 pos;
        public Quaternion rot;
    };
    
    /// <summary>
    /// This class is intended to handle the logic of spinning up any portals 
    /// and associated items.
    /// </summary>
    public class PortalCreator
    {
        public bool UseWebCam = true;
        public static Mesh cubeMesh;

        static PortalCreator()
        {
            // Cache the cube mesh for easy access to generate the portal 
            // mesh later
            var tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMesh = tempCube.GetComponent<MeshFilter>().mesh;
            GameObject.Destroy(tempCube);
        }

        public PortalCreator() { }

        public GameObject GeneratePortal()
        {
            // Determine where you want to generate the portal
            // Determine the orientation of the portal
            // Generate the portal object
            // Make the portal invisible
            // Generate the portal camera
            // Set the portal camera settings
            // Assign the portal camera to the portal object
            // Assign the portal feed to the camera
            // Make the portal object visible
            
            GameObject portal = generatePortalObject(UseWebCam);
            SetAsActivePortal(portal);

            return portal;
        }

        public bool SetAsActivePortal(GameObject portal)
        {
            if (portal != null)
            {
                GameObject mainCam = getMainCamera();
                UVCalc camTracker = mainCam.GetComponent<UVCalc>();
                if (camTracker == null)
                {
                    // If the camera hasn't initialized as a portal camera, make 
                    // it a portal camera
                    camTracker = mainCam.AddComponent<UVCalc>();
                }

                GameObject portalCamObj = portal.transform.GetChild(0).gameObject;
                camTracker.PortalCamera = portalCamObj;

                ShaderSetter shaderSetter = portal.GetComponent<ShaderSetter>();
                if (shaderSetter != null)
                {
                    shaderSetter.Viewer = mainCam;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private GameObject generatePortalObject(bool webcamFeed)
        {
            // Make the portal and disable it so it doesn't show immediately 
            // while loading
            GameObject portal = new GameObject();
            portal.SetActive(false);
            portal.name = "Portal";
            setPortalSettings(portal);

            // Set the portal's pose
            PortalTransform pt = getPortalTransform();
            portal.transform.SetPositionAndRotation(pt.pos, pt.rot);

            // Set the camera used to determine settings for the portal
            GameObject portalCam = generatePortalCamera();
            portalCam.transform.SetParent(portal.transform);

            // Attach custom scripts
            portal.AddComponent<BoundCalc>();
            ShaderSetter shaderSetter = portal.AddComponent<ShaderSetter>();
            // Set ShaderSetter's main camera for easy reference
            shaderSetter.Viewer = getMainCamera();

            if (webcamFeed)
            {
                GameObject webcamObj = GameObject.Find("WebCamManager");
                var webcamManager = webcamObj.GetComponent<WebCamManager>();
                webcamManager.Portal = portal;
            }

            // Reactivate the portal to make it visible now that everything
            // has loaded
            portal.SetActive(true);

            return portal;
        }

        private void setPortalSettings(GameObject portal)
        {
            // Set the mesh filter and mesh renderer to be a cube
            var mf = portal.AddComponent<MeshFilter>();
            var mr = portal.AddComponent<MeshRenderer>();
            mf.mesh = cubeMesh;
            mr.material = generatePortalMaterial();

            // Flatten the cube to give a square portal
            portal.transform.localScale = new Vector3(1, 1, 0.01f);
        }

        private Material generatePortalMaterial()
        {
            Material portalMat = new Material(Shader.Find("Custom/PortalShader"));
            portalMat.name = "PortalMaterial";
            return portalMat;
        }

        private GameObject generatePortalCamera()
        {
            GameObject portalCamera = new GameObject();
            portalCamera.name = "PortalCam";
            portalCamera.AddComponent<Camera>();

            setPortalCameraSettings(portalCamera);

            // Disable audio listener for additional cameras to avoid glitches 
            // with Unity
            AudioListener audioListener = portalCamera.GetComponent<AudioListener>();
            if(audioListener != null)
            {
                audioListener.enabled = false;
            }

            return portalCamera;
        }

        private void setPortalCameraSettings(GameObject portalCamera)
        {
            Camera portalCam = portalCamera.GetComponent<Camera>();
            if(portalCam == null)
            {
                Debug.LogError("Portal camera object does not contain camera component.");
            }
            else
            {
                portalCam.nearClipPlane = 0.3f;
                portalCam.farClipPlane = 1000f;
                portalCam.fieldOfView = 120;
                portalCam.depth = -1;
            }
        }

        /// <summary>
        /// [deprecated] Not currently used.
        /// </summary>
        /// <param name="portalCam"></param>
        private void disableCamerasOtherThan(Camera portalCam)
        {
            Camera[] allCams = Camera.allCameras;
            for(int i = 0; i < allCams.Length; i++)
            {
                if(allCams[i] != portalCam)
                {
                    allCams[i].enabled = false;
                }
            }
        }
        
        private PortalTransform getPortalTransform()
        {
            // Get the phone's camera position and direction
            GameObject tangoCam = getMainCamera();
            Vector3 tangoCamPos = tangoCam.transform.position;
            Vector3 tangoCamDir = tangoCam.transform.forward;

            // Get the room mesh
            Mesh roomMesh = getRoomMesh();

            // Get the ray collision
            RaycastHit hit = getRayCollision(tangoCamPos, tangoCamDir, roomMesh);

            // Assign the portal's position to be 0.05m closer to the camera 
            // than the room mesh wall
            Vector3 portalPos = hit.point - (tangoCamDir * 0.05f);

            // Set the portal's orientation to be the same as the camera's
            Quaternion portalRot = tangoCam.transform.rotation;

            PortalTransform portalTransform = new PortalTransform();
            portalTransform.pos = portalPos;
            portalTransform.rot = portalRot;

            return portalTransform;
        }
        
        private Mesh getRoomMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Color32> colors = new List<Color32>();
            List<int> triangles = new List<int>();

            var tangoApp = GameObject.Find("Tango Manager").GetComponent<TangoApplication>();

            if (tangoApp.Tango3DRExtractWholeMesh(vertices, normals, colors, triangles) == Tango3DReconstruction.Status.SUCCESS)
            {
                // if successful at extracting mesh...
                Mesh m = new Mesh();
                m.SetVertices(vertices);
                m.SetTriangles(triangles, 0);
                m.RecalculateNormals();

                return m;
            }
            else
            {
                Debug.LogError("Tango unable to extract mesh");
                return null;
            }
        }
        
        private RaycastHit getRayCollision(Vector3 origin, Vector3 direction, Mesh m)
        {
            Ray ray = new Ray(origin, direction);
            RaycastHit hit;

            Physics.Raycast(ray, out hit);

            return hit;
        }

        private GameObject getMainCamera()
        {
            return GameObject.Find("Tango Camera");
        }
    }
}