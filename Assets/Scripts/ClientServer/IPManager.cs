using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace ARPortal
{
    public class IPManager : MonoBehaviour
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily.ToString() == "InterNetwork")
                {
                    return ip.ToString();
                }
            }
            return null;
        }
    }
}