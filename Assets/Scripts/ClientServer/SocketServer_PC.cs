using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ARPortal
{
    public class SocketServer_PC : Socket_Base_PC
    {
#if !UNITY_WSA_10_0
        public static int numListeners = 15;
        protected static TcpListener listener;

        public static WebCamTexture[] feeds;
        public static int[] widths;
        public static int[] heights;
        public static List<Color[]> pixelsList;
        public static string[] names;

        // Thread signal for client connection
        //public static ManualResetEvent clientConnected = new ManualResetEvent(false);

        public static class Messages
        {
            public static class Errors
            {
                public static string ListenerNotPending = "TCPListener is uninitialized";//"TCPListener is either uninitialized or has no clients waiting to send/request messages.";
                public static string SendFileFailed = "Sending of file data failed. File not found.";
                public static string SendDataFailed = "Sending of data failed. Byte array is zero length or null.";
                public static string ReceiveDataFailed = "Data stream was empty.";
            }
        }

        public static void Start()
        {
            UpdateCachedFeeds();

            int port = Port.ClientServerConnection.PortNumber;
            listener = new TcpListener(IPAddress.Any, port);
            // Bind listener
            EndPoint localEndpoint = new IPEndPoint(IPAddress.Any, port);
            listener.Server.Bind(localEndpoint);
            // Begin listening
            listener.Server.Listen(numListeners);
            // Start accepting the socket
            listener.BeginAcceptSocket(new AsyncCallback(AcceptSocketCallback), listener);
        }

        public static void UpdateCachedFeeds()
        {
            feeds = GameObject.Find("WebCamManager").GetComponent<WebCamManager>().VideoFeeds;
            widths = new int[feeds.Length];
            heights = new int[feeds.Length];
            pixelsList = new List<Color[]>();
            names = new string[feeds.Length];

            for (int i = 0; i < feeds.Length; i++)
            {
                widths[i] = feeds[i].width;
                heights[i] = feeds[i].height;
                pixelsList.Add(feeds[i].GetPixels());
                names[i] = feeds[i].name;
            }
        }

        public static void AcceptSocketCallback(IAsyncResult ar)
        {
            Debug.Log("Socket accepted! Attempting to process...");

            // Retrieve the listener
            TcpListener listener = (TcpListener)ar.AsyncState;
            listener.BeginAcceptSocket(new AsyncCallback(AcceptSocketCallback), listener);

            // Accept the socket
            Socket clientSocket = listener.EndAcceptSocket(ar);
            Debug.Log("Socket finalized and accepted!");
            int clientPort = ((IPEndPoint)clientSocket.RemoteEndPoint).Port;
            Debug.Log("client port is " + clientPort);

            if (clientPort == Port.SendWebCamData.PortNumber)
            {
                List<Texture2D> textureList = new List<Texture2D>();
                // Get all of the web textures
                //WebCamTexture[] feeds = GameObject.Find("WebCamManager").GetComponent<WebCamManager>().VideoFeeds;
                for (int i = 0; i < feeds.Length; i++)
                {
                    //SendTexture(feeds[i], clientSocket);
                    SendTexture(widths[i], heights[i], pixelsList[i], names[i], clientSocket);
                }
            }
        }

        ~SocketServer_PC()
        {
            listener.Server.Disconnect(false);
            listener.Server.Close();
            listener.Stop();
        }
#endif
    }
}