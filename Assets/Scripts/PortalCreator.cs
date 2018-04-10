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
    public class PortalCreator : MonoBehaviour
    {
        public string MainCameraName = "Tango Camera";
        public PortalManager portalManager;
        public static Mesh cubeMesh;

        public void CacheCubeMesh()
        {
            // Cache the cube mesh for easy access to generate the portal 
            // mesh later
            var tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMesh = tempCube.GetComponent<MeshFilter>().mesh;
            GameObject.Destroy(tempCube);
        }

        public GameObject GeneratePortal()
        {
            GameObject portal = generatePortalObject();
            //SetAsActivePortal(portal);
            portalManager.RegisterPortal(portal);

            return portal;
        }

        private GameObject generatePortalObject()
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
            //shaderSetter.Viewer = MainCamera;

            //GameObject webcamObj = GameObject.Find("WebCamManager");
            //var webCamManager = webcamObj.GetComponent<WebCamManager>();
            //webCamManager.Portal = portal;
            portal.AddComponent<PortalScript>();
            UVCalc uvCalc = portal.AddComponent<UVCalc>();
            uvCalc.Viewer = GetMainCamera();
            uvCalc.PortalCamera = portalCam;

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
            if(cubeMesh == null)
            {
                CacheCubeMesh();
            }
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
        
        private PortalTransform getPortalTransform()
        {
            // Get the phone's camera position and direction
            GameObject mainCam = GetMainCamera();
            if(GetMainCamera() == null)
            {
                Debug.LogError("MainCamera is NULL!");
            }
            Debug.LogWarning("Main camera can't be found if assigned. Why?");
            Vector3 tangoCamPos = mainCam.transform.position;
            Vector3 tangoCamDir = mainCam.transform.forward;

            // Get the room mesh
            Mesh roomMesh = getRoomMesh();

            // Get the ray collision
            RaycastHit hit = getRayCollision(tangoCamPos, tangoCamDir, roomMesh);

            // Assign the portal's position to be 0.05m closer to the camera 
            // than the room mesh wall
            Vector3 portalPos = hit.point - (tangoCamDir * 0.05f);

            Debug.LogError("Assigning static portal position for testing purposes only!");
            portalPos = new Vector3(0, 0, 0.5f);

            // Set the portal's orientation to be the same as the camera's
            Quaternion portalRot = mainCam.transform.rotation;

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
        
        public GameObject GetMainCamera()
        {
            return GameObject.Find(MainCameraName);
        }
    }
}