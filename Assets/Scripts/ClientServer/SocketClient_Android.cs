using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ARPortal
{
    public class SocketClient_Android : Socket_Base_PC
    {
        public static List<Texture2D> ReceivedTextureList;
        private static Semaphore _pool;

        static SocketClient_Android()
        {
            ReceivedTextureList = new List<Texture2D>();
            _pool = new Semaphore(0, 1);
            //Thread.Sleep(500); // Wait for half a second to allow all threads to start and block on semaphore
        }

#if !UNITY_WSA_10_0
        public static List<Texture2D> RequestData(int port)
        {
            return RequestData(ServerFinder.serverIP, port);
        }

        public static List<Texture2D> RequestData(string serverIP, int port)
        {
            new Thread(() =>
            {
                // Generate the socket
                //TcpClient tcp = new TcpClient();
                //IPAddress clientIP = IPAddress.Parse(IPManager.ExtractIPAddress(IPManager.CompileNetworkConfigString(port)));
                IPAddress clientIP = IPAddress.Parse(IPManager.GetLocalIPAddress());
                TcpClient client = new TcpClient(new IPEndPoint(clientIP, port));

                // Connect to the server
                int serverPort = Port.ClientServerConnection.PortNumber;
                IPAddress serverIPAddress = IPAddress.Parse(serverIP);

                client.Connect(serverIPAddress, serverPort);

                // After awaiting the connection, receive data appropriately
                Socket socket = client.Client;
                _pool.WaitOne();
                ReceivedTextureList = ReceiveTexture(socket);
                _pool.Release();

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }).Start();

            _pool.WaitOne();
            Texture2D[] texArr = new Texture2D[ReceivedTextureList.Count];
            ReceivedTextureList.CopyTo(texArr);
            _pool.Release(1);

            return new List<Texture2D>(texArr);
        }

        public static void ReleaseSemaphores()
        {
            if(_pool != null)
            {
                _pool.Release(1);
            }
        }

        ////public static void ConnectTest(string serverNetworkConfig, int port)
        //public static void ConnectTest(int port)
        //{
        //    new Thread(() =>
        //    {
        //        // Generate the socket
        //        TcpClient tcp = new TcpClient();

        //        // Connect to the server
        //        int serverPort = Config.Ports.ClientServerConnection;
        //        //IPAddress serverIP = IPAddress.Parse(IPManager.ExtractIPAddress(serverNetworkConfig));
        //        IPAddress serverIP = IPAddress.Any;
        //        tcp.Connect(serverIP, serverPort);

        //        // After awaiting the connection, receive data appropriately
        //        Socket socket = tcp.Client;
        //        byte[] data = new byte[1024];
        //        int numBytesReceived = 0;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            while ((numBytesReceived = socket.Receive(data, 1024, SocketFlags.None)) > 0)
        //            {
        //                ms.Write(data, 0, numBytesReceived);
        //                Debug.Log("Data received! Size = " + numBytesReceived);
        //            }
        //            Debug.Log("Finished receiving data: size = " + ms.Length);

        //            byte[] allData = ms.ToArray();

        //            // Clean up socket & close connection
        //            ms.Close();
        //            socket.Shutdown(SocketShutdown.Both);
        //            socket.Close();
        //        }
        //    }).Start();
        //}

        //// Assumes that all information will be separated by lengths of data and that it will be one long continuous stream of data
        //public static void ConnectTest2(int port)
        //{
        //    new Thread(() =>
        //    {
        //        // Generate the socket
        //        TcpClient tcp = new TcpClient();

        //        // Connect to the server
        //        int serverPort = Config.Ports.ClientServerConnection;
        //        //IPAddress serverIP = IPAddress.Parse(IPManager.ExtractIPAddress(serverNetworkConfig));
        //        IPAddress serverIP = IPAddress.Any;
        //        tcp.Connect(serverIP, serverPort);

        //        // After awaiting the connection, receive data appropriately
        //        Socket socket = tcp.Client;
        //        int bufferLength = 1024;
        //        byte[] data = new byte[bufferLength];
        //        int numBytesReceived = 0;
        //        //using (MemoryStream ms = new MemoryStream())
        //        //{
        //        //while ((numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None)) > 0)
        //        //{
        //        //    ms.Write(data, 0, numBytesReceived);
        //        //    Debug.Log("Data received! Size = " + numBytesReceived);
        //        //}
        //        //Debug.Log("Finished receiving data: size = " + ms.Length);

        //        //byte[] allData = ms.ToArray();

        //        // Determine the number of files and names of files to transfer in
        //        ////string dataHeader = System.Text.Encoding.UTF8.GetString(allData);



        //        //int numParses;
        //        //if(!int.TryParse(dataHeader.Split(new char[1] { ';' })[0], out numParses))
        //        //{
        //        //    Debug.Log("Data header parsing failed! Unable to determine number of files to transfer.");
        //        //    //socket.Close();
        //        //}

        //        using (MemoryStream fileStream = new MemoryStream())
        //        {
        //            int headerIndex = 0;
        //            //string filename = dataHeader.Split(';')[headerIndex++];
        //            int dataLengthIndex = 0;
        //            //int numReceives = 0;

        //            // Get directory to save it to
        //            string directory = "C:\\Users\\Thomas\\Documents\\tempwritefile";

        //            string dataHeader = string.Empty;

        //            do
        //            {
        //                int numBytesAvailable = bufferLength;
        //                int dataIndex = 0;

        //                // Get the first receive from the socket
        //                numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);

        //                // If there are any bytes that continue a file from the last buffer read, handle that here
        //                if (dataLengthIndex > 0 && dataLengthIndex < bufferLength)
        //                {
        //                    fileStream.Write(data, 0, dataLengthIndex);
        //                    string filename = dataHeader.Split(';')[headerIndex++];
        //                    File.WriteAllBytes(Path.Combine(directory, filename), fileStream.ToArray());
        //                }

        //                // While there are file pieces we can get from the gathered data,
        //                // determine where the bytes designating the lengths of files about to be
        //                // transferred over are and then grab the file lengths and file bytes
        //                while (dataLengthIndex < bufferLength)
        //                {
        //                    // Get the 4 bytes indicating the length
        //                    int dataLength = 0;
        //                    if (dataLengthIndex <= bufferLength - 4)
        //                    {
        //                        // If length is shown fully within the buffer (i.e. length bytes aren't split between reads)...
        //                        dataLength = System.BitConverter.ToInt32(data, dataLengthIndex);
        //                        //if(dataLengthIndex == bufferLength - 4)
        //                        //{
        //                        //    // Handle special case of pulling 
        //                        //    numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);
        //                        //    dataLengthIndex -= bufferLength;

        //                        //    continue;
        //                        //}
        //                    }
        //                    else
        //                    {
        //                        // Else length bytes are split between reads...
        //                        byte[] dataLengthBuffer = new byte[4];
        //                        int numDataLengthBytesCopied = bufferLength - dataLengthIndex;
        //                        System.Buffer.BlockCopy(data, dataLengthIndex, dataLengthBuffer, 0, numDataLengthBytesCopied);
        //                        numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);
        //                        System.Buffer.BlockCopy(data, 0, dataLengthBuffer, numDataLengthBytesCopied, 4 - numDataLengthBytesCopied);

        //                        dataLength = System.BitConverter.ToInt32(dataLengthBuffer, 0);
        //                        dataLengthIndex -= bufferLength;
        //                    }
        //                    dataIndex = dataLengthIndex + 4;
        //                    dataLengthIndex = dataIndex + dataLength; // Update the data length index for the while loop check
        //                    numBytesAvailable = numBytesReceived - dataIndex;

        //                    // Handle instances where whole file is contained in part of buffer
        //                    if (dataIndex + dataLength < numBytesAvailable)
        //                    {
        //                        byte[] fileData = new byte[dataLength];
        //                        System.Buffer.BlockCopy(data, dataIndex, fileData, 0, dataLength);
        //                        if (dataHeader.Equals(string.Empty))
        //                        {
        //                            // If the header hasn't been received yet
        //                            dataHeader = System.Text.Encoding.UTF8.GetString(fileData);
        //                        }
        //                        else
        //                        {
        //                            // If the header's been received, that means we're looking at actual file data
        //                            string filename = dataHeader.Split(';')[headerIndex++];
        //                            File.WriteAllBytes(Path.Combine(directory, filename), fileData);
        //                        }

        //                        //// Determine 
        //                        //if (numBytesAvailable < dataLength)
        //                        //{
        //                        //    fileStream.Write(data, dataIndex, numBytesAvailable);
        //                        //}
        //                        //else
        //                        //{
        //                        //    byte[] fileData = new byte[dataLength];
        //                        //    System.Buffer.BlockCopy(data, dataIndex, fileData, 0, dataLength);
        //                        //    File.WriteAllBytes(Path.Combine(directory, filename), fileData);

        //                        //    // Get the next filename
        //                        //    filename = dataHeader.Split(';')[headerIndex++];

        //                        //    dataLengthIndex += 4 + dataLength;
        //                        //}
        //                    }
        //                }

        //                // Write remainder of bytes in buffer to the file memory stream to store for the next buffer read
        //                fileStream.Write(data, dataIndex, numBytesAvailable);
        //                dataLengthIndex -= bufferLength;
        //                // continue;


        //            } while (numBytesReceived == bufferLength);

        //            fileStream.Close();
        //        }


        //        //    int numParses = dataHeader.Split(new char[1] { ';' }).Length; 
        //        ////if(numParses < 1)
        //        ////{
        //        ////    Debug.Log("Data header parsing failed! Unable to determine number of files to transfer.");
        //        ////    //socket.Close();
        //        ////}
        //        ////else
        //        ////{
        //        //for (int parseIndex = 0; parseIndex < numParses; parseIndex++)
        //        //{
        //        //    string filename = dataHeader.Split(new char[1] { ';' })[parseIndex];
        //        //    numBytesReceived = 0;
        //        //    using (MemoryStream fileStream = new MemoryStream())
        //        //    {
        //        //        do
        //        //        {
        //        //            numBytesReceived = socket.Receive(data, 1024, SocketFlags.None);
        //        //            fileStream.Write(data, 0, numBytesReceived);

        //        //            Debug.Log("Data received! Size = " + numBytesReceived);
        //        //        } while ((numBytesReceived == 1024));

        //        //        string filepath = Path.Combine("C:\\Users\\Thomas\\Documents\\tempwritefile", filename);

        //        //        File.WriteAllBytes(filepath, fileStream.ToArray());

        //        //        fileStream.Close();
        //        //    }
        //        //}
        //        ////}

        //        // Clean up socket & close connection
        //        //ms.Close();
        //        socket.Shutdown(SocketShutdown.Both);
        //        socket.Close();
        //        //}
        //    }).Start();
        //}
#endif
    }
}