using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ARPortal
{
    public class ServerFinder
    {
        public static string serverIP;
        private static Thread thread_AcceptClient;
        public static string displayMsg = "";

#if !UNITY_WSA_10_0
        public static UdpClient listener;

        public static void ServerStart()
        {
            int listenerPort = Port.FindServer.PortNumber;
            listener = new UdpClient(listenerPort);
            serverIP = IPManager.GetLocalIPAddress().ToString();

            AcceptClient();
        }

        public static void AcceptClient()
        {
            byte[] serverIPBytes = Encoding.UTF8.GetBytes(serverIP);

            if (thread_AcceptClient != null
                && thread_AcceptClient.IsAlive)
            {
                thread_AcceptClient.Abort();
            }

            thread_AcceptClient = new Thread(() =>
            {
                while (true)
                {
                    IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    // ERROR TESTING - NEED TO ASSIGN THE SYSTEM A PORT # THAT WORKS, INSTEAD OF 0 ABOVE
                    byte[] clientIPBytes = listener.Receive(ref clientEndpoint);
                    //string clientIPString = Encoding.UTF8.GetString(clientIPBytes);
                    listener.Send(serverIPBytes, serverIPBytes.Length, clientEndpoint);
                    displayMsg = "Client accepted!";
                    MultiThreadDebug.Log("Client accepted! (" + clientEndpoint.Address + ")");
                }
            });

            thread_AcceptClient.Start();
        }

        // IPAddress string
        public static string FindServer()
        {
            string IPString = string.Empty;

            UdpClient client = new UdpClient();
            Debug.Log("Checkpoint 1");
            client.EnableBroadcast = true;
            int findServerPort = Port.FindServer.PortNumber;
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Broadcast, findServerPort);
            //IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("192.168.2.2"), findServerPort);
            Debug.Log("Checkpoint 2");
            byte[] clientIPBytes = Encoding.UTF8.GetBytes(IPManager.GetLocalIPAddress().ToString());
            client.Send(clientIPBytes, clientIPBytes.Length, serverEndpoint);
            byte[] serverIPBytes = client.Receive(ref serverEndpoint);

            Debug.Log("Checkpoint 3");

            IPString = Encoding.UTF8.GetString(serverIPBytes);
            serverIP = IPString;

            Debug.Log("Checkpoint 4");

            client.Close();

            return IPString;
        }

        public static bool KillThreads()
        {
            if (thread_AcceptClient != null
                && thread_AcceptClient.IsAlive)
            {
                if (listener != null)
                {
                    listener.Close();
                    listener = null;
                }
                thread_AcceptClient.Abort();
                return thread_AcceptClient.IsAlive;
            }

            return true;
        }
#endif
    }
}