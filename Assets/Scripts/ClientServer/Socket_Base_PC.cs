using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System; // BitConverter

namespace ARPortal
{
    public class Socket_Base_PC
    {
#if !UNITY_WSA_10_0
        public static void SendTexture(Texture2D tex, Socket socket)
        {
            TexturePacket packet = new TexturePacket(tex);
            socket.Send(packet.bytes);
            // Do we shut down for this implementation?
            //socket.Shutdown(SocketShutdown.Both);
        }

        public static void SendTexture(WebCamTexture feed, Socket socket)
        {
            TexturePacket packet = new TexturePacket(feed);
            socket.Send(packet.bytes);
        }

        public static List<Texture2D> ReceiveTexture(Socket socket)
        {
            List<Texture2D> texList = new List<Texture2D>();
            List<MemoryStream> msList = ReadStream(socket);
            for (int i = 0; i < msList.Count; i++)
            {
                Texture2D tex = TexturePacket.ReadFromBytes(msList[i].ToArray());
                texList.Add(tex);
            }
            return texList;
        }

        public static List<MemoryStream> ReadStream(Socket socket)
        {
            List<MemoryStream> msList = new List<MemoryStream>();
            int bufferLength = 1024;
            byte[] buffer = new byte[bufferLength];
            bool packetInitiated = false;
            int textureIndex = 0;

            MemoryStream ms = new MemoryStream();
            int bytesRemainingForPacket = 0;
            while (true)
            {
                int numBytesReceived = socket.Receive(buffer, bufferLength, SocketFlags.None);
                ms.Write(buffer, 0, numBytesReceived);

                if (numBytesReceived <= 0
                    && bytesRemainingForPacket == 0)
                {
                    break;
                }

                int appendIndex = 0;
                bool wasPreviouslyInitiated = packetInitiated;
                List<MemoryStream> appendList = ProcessTexturePacket(ref packetInitiated, ref bytesRemainingForPacket, numBytesReceived, buffer);
                if (wasPreviouslyInitiated)
                {
                    // Add first memory stream to existing memory stream
                    MemoryStream previousMS = msList[textureIndex];
                    previousMS.Write(appendList[0].ToArray(), 0, appendList[0].ToArray().Length);
                    appendIndex++;
                }
                // Add other memory streams
                for (; appendIndex < appendList.Count; appendIndex++)
                {
                    msList.Add(appendList[appendIndex]);
                    textureIndex++;
                }
            }

            return msList;
        }

        /// <summary>
        /// List will be null if nothing was read in. Or could be a list of multiple memory streams for multiple packets
        /// </summary>
        /// <returns></returns>
        public static List<MemoryStream> ProcessTexturePacket(ref bool packetInitiated, ref int bytesRemainingForPacket, int numBytesAvailable, byte[] buffer)
        {
            List<MemoryStream> msList = new List<MemoryStream>();
            MemoryStream ms = new MemoryStream();
            int byteIndex = 0;

            while (numBytesAvailable > 0)
            {
                if (!packetInitiated)
                {
                    bytesRemainingForPacket = BitConverter.ToInt32(buffer, byteIndex);
                    // Don't increment the byteIndex, because it still needs to 
                    // get processed by TexturePacket class, so we don't want 
                    // to pass over the headerSize int
                    ms = new MemoryStream();
                    packetInitiated = !packetInitiated;
                }
                // Read the bytes
                if (bytesRemainingForPacket > numBytesAvailable)
                {
                    ms.Write(buffer, byteIndex, numBytesAvailable);
                    msList.Add(ms);
                    byteIndex += numBytesAvailable;
                    numBytesAvailable -= numBytesAvailable;
                    break;
                }
                else if(bytesRemainingForPacket == numBytesAvailable)
                {
                    ms.Write(buffer, byteIndex, numBytesAvailable);
                    msList.Add(ms);
                    byteIndex += numBytesAvailable;
                    numBytesAvailable -= numBytesAvailable;
                    packetInitiated = false;
                    break;
                }
                else
                {
                    // bytes available after writing
                    ms.Write(buffer, byteIndex, bytesRemainingForPacket);
                    msList.Add(ms);
                    byteIndex += bytesRemainingForPacket;
                    numBytesAvailable -= bytesRemainingForPacket;
                    packetInitiated = false;
                    // repeat
                }
            }

            return msList;
        }

        //public static void PrepSocketData(Texture2D[] textures, ref MemoryStream ms)
        //{
        //    string header = BuildSocketHeader(textures);
        //    byte[] headerData = StringToBytes(header);

        //}

        //public static void SendFile(string filepath, Socket socket)
        //{
        //    SendFiles(new string[1] { filepath }, socket);
        //}

        //public static void SendFiles(string[] filepaths, Socket socket)
        //{
        //    // Needs to tell the client socket what the server's ip is
        //    //string configString = IPManager.CompileNetworkConfigString(Config.Ports.ClientServerConnection);

        //    foreach (string filepath in filepaths)
        //    {
        //        UnityEngine.Debug.Log("Sending " + Path.GetFileName(filepath));
        //    }

        //    MemoryStream ms = new MemoryStream();
        //    PrepSocketData(filepaths, ref ms);
        //    socket.Send(ms.ToArray());
        //    ms.Close();
        //    ms.Dispose();
        //    socket.Shutdown(SocketShutdown.Both);
        //}

        //public static void PrepSocketData(string[] filepaths, ref MemoryStream ms)
        //{
        //    string header = BuildSocketHeader(filepaths);

        //    //byte[] headerData = System.Text.Encoding.UTF8.GetBytes(header);
        //    byte[] headerData = StringToBytes(header);
        //    // Add header data length
        //    ms.Write(System.BitConverter.GetBytes(headerData.Length), 0, System.BitConverter.GetBytes(headerData.Length).Length);
        //    // Add header data
        //    ms.Write(headerData, 0, headerData.Length);

        //    foreach (string filepath in filepaths)
        //    {
        //        byte[] fileData = File.ReadAllBytes(filepath);
        //        // Add file data length
        //        ms.Write(System.BitConverter.GetBytes(fileData.Length), 0, System.BitConverter.GetBytes(fileData.Length).Length);
        //        // Add file data
        //        ms.Write(fileData, 0, fileData.Length);
        //    }
        //}

        //public static string BuildSocketHeader(string[] filepaths)
        //{
        //    System.Text.StringBuilder headerBuilder = new System.Text.StringBuilder();

        //    foreach (string filepath in filepaths)
        //    {
        //        headerBuilder.Append(Path.GetFileName(filepath));
        //        headerBuilder.Append(';');
        //    }
        //    headerBuilder.Remove(headerBuilder.Length - 1, 1); // Remove the last separator (';')

        //    return headerBuilder.ToString();
        //}

        //public static void ReceiveFiles(Socket socket, string receiveDirectory)
        //{
        //    if (!Directory.Exists(receiveDirectory))
        //    {
        //        AbnormalDirectoryHandler.CreateDirectory(receiveDirectory);
        //    }

        //    int bufferLength = 1024;
        //    byte[] data = new byte[bufferLength];
        //    int numBytesReceived = 0;

        //    MemoryStream fileStream = new MemoryStream();

        //    int headerIndex = 0;
        //    int dataLengthIndex = 0;
        //    string dataHeader = string.Empty;

        //    do
        //    {
        //        // Get the first receive from the socket
        //        numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);
        //        int numBytesAvailable = numBytesReceived;
        //        int dataIndex = 0;

        //        // If there are any bytes that continue a file from the last buffer read, handle that here
        //        if (dataLengthIndex > 0 && dataLengthIndex < numBytesReceived)
        //        {
        //            fileStream.Write(data, 0, dataLengthIndex);
        //            string filename = dataHeader.Split(';')[headerIndex++];
        //            File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());
        //            // MemoryStream flush does literally nothing.
        //            fileStream.Close();
        //            fileStream.Dispose();
        //            fileStream = new MemoryStream();
        //        }
        //        else if (numBytesReceived <= 0)
        //        {
        //            string filename = dataHeader.Split(';')[headerIndex++];
        //            File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());
        //            // MemoryStream flush does literally nothing.
        //            fileStream.Close();
        //            fileStream.Dispose();
        //            fileStream = new MemoryStream();
        //        }

        //        // While there are file pieces we can get from the gathered data,
        //        // determine where the bytes designating the lengths of files about to be
        //        // transferred over are and then grab the file lengths and file bytes
        //        while (dataLengthIndex >= 0 && dataLengthIndex < numBytesReceived)
        //        {
        //            // Get the 4 bytes indicating the length
        //            int dataLength = 0;
        //            if (dataLengthIndex <= numBytesReceived - 4)
        //            {
        //                // If length is shown fully within the buffer (i.e. length bytes aren't split between reads)...
        //                dataLength = System.BitConverter.ToInt32(data, dataLengthIndex);

        //                if (dataLength <= 0)
        //                {
        //                    // Handle case where end of stream is reached
        //                    break;
        //                }
        //            }
        //            //else
        //            else if (dataLengthIndex < numBytesReceived && dataLengthIndex > numBytesReceived - 4)
        //            {
        //                // Else length bytes are split between reads...
        //                byte[] dataLengthBuffer = new byte[4];
        //                int numDataLengthBytesCopied = numBytesReceived - dataLengthIndex;
        //                System.Buffer.BlockCopy(data, dataLengthIndex, dataLengthBuffer, 0, numDataLengthBytesCopied);
        //                numBytesReceived = socket.Receive(data, bufferLength, SocketFlags.None);
        //                numBytesAvailable = numBytesReceived;
        //                System.Buffer.BlockCopy(data, 0, dataLengthBuffer, numDataLengthBytesCopied, 4 - numDataLengthBytesCopied);

        //                dataLength = System.BitConverter.ToInt32(dataLengthBuffer, 0);
        //                dataLengthIndex -= numBytesReceived;
        //            }
        //            dataIndex = dataLengthIndex + 4;
        //            dataLengthIndex = dataIndex + dataLength; // Update the data length index for the while loop check
        //            numBytesAvailable = numBytesReceived - dataIndex;

        //            // Handle instances where whole file is contained in part of buffer
        //            if (numBytesAvailable > 0 && dataIndex + dataLength < numBytesAvailable)
        //            {
        //                byte[] fileData = new byte[dataLength];
        //                System.Buffer.BlockCopy(data, dataIndex, fileData, 0, dataLength);
        //                if (dataHeader.Equals(string.Empty))
        //                {
        //                    // If the header hasn't been received yet
        //                    dataHeader = BytesToString(fileData);
        //                    //dataHeader = System.Text.Encoding.UTF8.GetString(fileData);
        //                }
        //                else
        //                {
        //                    // If the header's been received, that means we're looking at actual file data
        //                    string filename = dataHeader.Split(';')[headerIndex++];
        //                    File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileData);
        //                }
        //            }
        //        }

        //        // Write remainder of bytes in buffer to the file memory stream to store for the next buffer read
        //        if (numBytesAvailable < 0)
        //        {
        //            Debug.Log("TCP Error: Stream read logic error.");
        //            break;
        //        }
        //        else
        //        {
        //            fileStream.Write(data, dataIndex, numBytesAvailable);
        //            dataLengthIndex -= numBytesReceived;
        //        }
        //        // continue;

        //    } while (numBytesReceived > 0);

        //    fileStream.Close();
        //    fileStream.Dispose();
        //    //#if UNITY_EDITOR
        //    //            UnityEditor.AssetDatabase.Refresh();
        //    //#endif
        //}

        public static string BytesToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static byte[] StringToBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
#endif
    }
}