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
            GameObject window = generateWindow(portal);
            window.transform.parent = portal.transform;
            window.transform.localPosition = Vector3.zero;
            window.transform.localRotation = Quaternion.identity;

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

#if UNITY_ANDROID
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
            GameObject tempObj = new GameObject();
            Quaternion camRot = mainCam.transform.rotation;
            Quaternion portalRot = new Quaternion(camRot.x, camRot.y, camRot.z, camRot.w); // Make a copy of the rotation at this point in time
            Vector3 camUp = mainCam.transform.up;
            tempObj.transform.rotation = portalRot;
            tempObj.transform.Rotate(camUp, 180);
            portalRot = tempObj.transform.rotation;
            GameObject.Destroy(tempObj);
            
            //Quaternion portalRot = mainCam.transform.rotation;

            PortalTransform portalTransform = new PortalTransform();
            portalTransform.pos = portalPos;
            portalTransform.rot = portalRot;
#else
            Vector3 tangoCamUp = mainCam.transform.up;

            PortalTransform portalTransform = new PortalTransform();
            portalTransform.pos = tangoCamPos + tangoCamDir * 3.0f;
            //portalTransform.rot = Quaternion.identity;

            GameObject tempObj = new GameObject();
            tempObj.transform.Rotate(tangoCamUp, 180);
            portalTransform.rot = tempObj.transform.rotation;
            GameObject.Destroy(tempObj);
#endif

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

        private GameObject generateWindow(GameObject portal)
        {
            if (portal != null)
            {
                GameObject window = new GameObject();
                window.SetActive(false);
                window.name = "Window";

                // generate frame
                GameObject frame = generateWindowFrame(portal);
                frame.transform.parent = window.transform;
                frame.transform.localPosition = Vector3.zero;

                // generate stiles
                GameObject stiles = generateWindowStiles(portal);
                stiles.transform.parent = window.transform;
                stiles.transform.localPosition = Vector3.zero;
                
                window.SetActive(true);
                return window;
            }
            else
            {
                return null;
            }
        }

        private GameObject generateWindowFrame(GameObject portal)
        {
            if (portal != null)
            {
                MeshFilter mf = portal.GetComponent<MeshFilter>();
                if(mf != null)
                {
                    Mesh m = mf.mesh;
                    if(m != null)
                    {
                        GameObject frame = new GameObject();
                        frame.SetActive(false);
                        frame.name = "WindowFrame";

                        Vector3 extents = m.bounds.extents;

                        // Set the frame dimensions
                        float frameWidth = extents.x / 8.0f;
                        float frameDepth = frameWidth;
                        Color frameColor = new Color(60 / 255.0f, 40 / 255.0f, 0);
                        //Color frameColor = Color.blue;
                        float windowWidth = extents.x * 2;
                        float windowHeight = extents.y * 2;

                        // Get the frame pieces
                        GameObject leftFramePiece = generateWindowFramePiece(frameWidth, windowHeight + frameWidth, frameDepth, frameColor);
                        leftFramePiece.name = "LeftWindowFrame";
                        GameObject rightFramePiece = generateWindowFramePiece(frameWidth, windowHeight + frameWidth, frameDepth, frameColor); ;
                        rightFramePiece.name = "RightWindowFrame";
                        GameObject topFramePiece = generateWindowFramePiece(windowWidth + frameWidth, frameWidth, frameDepth, frameColor);
                        topFramePiece.name = "TopWindowFrame";
                        GameObject bottomFramePiece = generateWindowFramePiece(windowWidth + frameWidth, frameWidth, frameDepth, frameColor);
                        bottomFramePiece.name = "BottomWindowFrame";

                        // Make them the children of the frame object
                        leftFramePiece.transform.parent = frame.transform;
                        rightFramePiece.transform.parent = frame.transform;
                        topFramePiece.transform.parent = frame.transform;
                        bottomFramePiece.transform.parent = frame.transform;

                        // Arrange the pieces correctly
                        Vector3 right = portal.transform.right;
                        Vector3 up = portal.transform.up;
                        leftFramePiece.transform.localPosition = -right * windowWidth / 2.0f;
                        rightFramePiece.transform.localPosition = right * windowWidth / 2.0f;
                        topFramePiece.transform.localPosition = up * windowHeight / 2.0f;
                        bottomFramePiece.transform.localPosition = -up * windowHeight / 2.0f;

                        frame.SetActive(true);
                        return frame;
                    }
                }
            }

            return null;
        }

        private GameObject generateWindowFramePiece(float width, float height, float depth, Color frameColor)
        {
            GameObject windowFramePiece = new GameObject();
            windowFramePiece.SetActive(false);
            windowFramePiece.name = "WindowFramePiece";

            var mf = windowFramePiece.AddComponent<MeshFilter>();
            mf.mesh = cubeMesh;
            windowFramePiece.transform.localScale = new Vector3(width, height, depth);
            var mr = windowFramePiece.AddComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Standard"));
            mr.material.SetColor("_Color", frameColor);

            windowFramePiece.SetActive(true);
            return windowFramePiece;
        }

        private GameObject generateWindowStiles(GameObject portal)
        {
            if (portal!= null)
            {
                MeshFilter mf = portal.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    Mesh m = mf.mesh;
                    if (m != null)
                    {
                        GameObject stiles = new GameObject();
                        stiles.SetActive(false);
                        stiles.name = "WindowStiles";

                        Vector3 extents = m.bounds.extents;

                        // Set the frame dimensions
                        float stileWidth = extents.x / 10.0f;
                        float stileDepth = stileWidth;
                        Color stileColor = Color.white;
                        float windowWidth = extents.x * 2;
                        float windowHeight = extents.y * 2;

                        // Get the frame pieces
                        GameObject horizontalStile = generateWindowFramePiece(windowWidth, stileWidth, stileDepth, stileColor);
                        horizontalStile.name = "HorizontalStile";
                        GameObject verticalStile = generateWindowFramePiece(stileWidth, windowHeight, stileDepth, stileColor); ;
                        verticalStile.name = "VerticalStile";

                        // Make them the children of the frame object
                        horizontalStile.transform.parent = stiles.transform;
                        verticalStile.transform.parent = stiles.transform;

                        // Pieces are already arranged correctly

                        stiles.SetActive(true);
                        return stiles;
                    }
                }
            }

            return null;
        }
    }
}