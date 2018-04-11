using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class NetworkManager : MonoBehaviour
    {
        public bool DisableNetworking = false;
#if UNITY_ANDROID
        public PortalManager portalManager;
#endif

        public void Start()
        {
#if UNITY_ANDROID
            //Trigger serverfinder
            // Trigger client or server based on platform
            if (!DisableNetworking)
            {
                ServerFinder.FindServer();
                InvokeRepeating("TESTING", 1.0f, 5.0f);
                //Invoke("TESTING", 0.0f);
            }
#else
            // PC
            if(!DisableNetworking)
            {
                ServerFinder.ServerStart();
                SocketServer_PC.Start();
            }
#endif
        }

        public void Update()
        {
#if UNITY_ANDROID
            //if (!DisableNetworking)
            //{
            //    // Get updated textures
            //    //List<Texture2D> texList = SocketClient_Android.RequestData(Port.SendWebCamData.PortNumber);
            //    SocketClient_Android.RequestData(Port.SendWebCamData.PortNumber);
            //    List<Texture2D> texList = SocketClient_Android.GrabTextures();

            //    //Debug.Log("Texture list for NetworkManager has size " + texList.Count);

            //    // Update the textures being used for the portal
            //    portalManager.UpdateTextures(texList);
            //}
#else
            // Gather textures
            // Wait for texture requests
            // Let SocketClient / SocketServer deal with it
            if(!DisableNetworking){}
#endif

            // ERROR TESTING - Remove the displayMsg route with multithreaded debugging in ServerFinder and in Socket_Base_PC
            if (!string.IsNullOrEmpty(ServerFinder.displayMsg))
            {
                //Debug.Log(ServerFinder.displayMsg);
                //ServerFinder.displayMsg = "";
            }

            MultiThreadDebug.Flush();
        }

        public void OnApplicationQuit()
        {
            ServerFinder.KillThreads();
#if UNITY_ANDROID
            SocketClient_Android.ReleaseSemaphores();
#endif
        }

        public void TESTING()
        {
            if (!DisableNetworking)
            {
                // Get updated textures
                //List<Texture2D> texList = SocketClient_Android.RequestData(Port.SendWebCamData.PortNumber);
                SocketClient_Android.RequestData(Port.SendWebCamData.PortNumber);
                List<Texture2D> texList = SocketClient_Android.GrabTextures();

                //Debug.Log("Texture list for NetworkManager has size " + texList.Count);

                // Update the textures being used for the portal
                portalManager.UpdateTextures(texList);
            }
        }
    }
}