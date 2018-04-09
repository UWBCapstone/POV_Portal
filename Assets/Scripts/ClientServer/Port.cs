using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public enum PortTypes
    {
        FindServer,
        ClientServerConnection
    };

    public class Port
    {
        public int PortNumber;

        private const int Default_s = 20000;
        private const int FindServer_s = 21288;
        private const int ClientServerConnection_s = 21289;
        private const int SendWebCamData_s = 21290;

        public Port()
        {
            PortNumber = Default_s;
        }

        public Port(int number)
        {
            if (number < 1)
            {
                PortNumber = Default_s;
                Debug.LogError("Invalid port number assigned. Assigning default of " + Default_s + ".");
            }
            else
            { 
                PortNumber = number;
            }

        }

        public static Port FindServer
        {
            get
            {
                return new Port(FindServer_s);
            }
        }
        public static Port ClientServerConnection
        {
            get
            {
                return new Port(ClientServerConnection_s);
            }
        }
        public static Port SendWebCamData
        {
            get
            {
                return new Port(SendWebCamData_s);
            }
        }
    }
}