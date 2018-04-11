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
        public static int debugBlockID_ProcessStream = 3;
        public static int debugBlockID_ProcessBuffer = 2;
        public static int debugBlockID_GenerateTexture = 1;


#if !UNITY_WSA_10_0
        public static void SendTexture(Texture2D tex, Socket socket)
        {
            TexturePacket packet = new TexturePacket(tex);
            socket.Send(packet.bytes);
            // Do we shut down for this implementation?
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public static void SendTexture(WebCamTexture feed, Socket socket)
        {
            TexturePacket packet = new TexturePacket(feed);
            socket.Send(packet.bytes);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public static void SendTexture(int width, int height, Color[] pixels, string name, Socket socket)
        {
            TexturePacket packet = new TexturePacket(width, height, pixels, name);
            socket.Send(packet.bytes);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public static List<Texture2D> ReceiveTexture(Socket socket)
        {
            List<Texture2D> texList = new List<Texture2D>();
            List<MemoryStream> msList = ReadStream(socket);
            for (int i = 0; i < msList.Count; i++)
            {
                MultiThreadDebug.Log("Generating texture from memory stream [" + i + "]");
                Texture2D tex = TexturePacket.ReadFromBytes(msList[i].ToArray());
                texList.Add(tex);
                MultiThreadDebug.Log("Finished generating texture from memory stream for MS[" + i + "]");
            }
            ServerFinder.displayMsg = "Contained " + msList.Count.ToString() + " memorystreams";
            MultiThreadDebug.Log("Received " + msList.Count.ToString() + " textures");
            return texList;
        }

        public static List<MemoryStream> ReceiveTextureBytes(Socket socket)
        {
            MultiThreadDebug.Log("Receiving Texture bytes...", debugBlockID_GenerateTexture);
            return ReadStream(socket);
        }

        public static List<MemoryStream> ReadStream(Socket socket)
        {
            List<MemoryStream> msList = new List<MemoryStream>();
            int bufferLength = 1024;
            byte[] buffer = new byte[bufferLength];
            bool packetInitiated = false;
            int textureCount = 0;

            MemoryStream ms = new MemoryStream();
            int bytesRemainingForPacket = 0;
            int DEBUGCOUNTER = 0;
            while (true)
            {
                int numBytesReceived = socket.Receive(buffer, bufferLength, SocketFlags.None);
                ms.Write(buffer, 0, numBytesReceived);

                ServerFinder.displayMsg = "NumBytesReceived = " + numBytesReceived.ToString() + "(" + DEBUGCOUNTER + ")";
                DEBUGCOUNTER += numBytesReceived;
                MultiThreadDebug.Log("Received packet!...NumBytesReceived = " + numBytesReceived.ToString() + "(total received = " + DEBUGCOUNTER + ")", debugBlockID_ProcessStream);

                //if (numBytesReceived <= 0
                //    && bytesRemainingForPacket == 0)
                //{
                //    ServerFinder.displayMsg = "Breaking out of while loop!";
                //    MultiThreadDebug.Log("Breaking out of packet reading while loop!");
                //    break;
                //}

                int appendIndex = 0;
                bool wasPreviouslyInitiated = packetInitiated;
                List<MemoryStream> appendList = ProcessTexturePacket(ref packetInitiated, ref bytesRemainingForPacket, numBytesReceived, buffer);
                if (wasPreviouslyInitiated)
                {
                    MultiThreadDebug.Log("Packet previously initiated! Appending to previous memorystream...", debugBlockID_ProcessStream);
                    // Add first memory stream to existing memory stream
                    MemoryStream previousMS = msList[textureCount-1];
                    previousMS.Write(appendList[0].ToArray(), 0, appendList[0].ToArray().Length);
                    appendIndex++;
                    MultiThreadDebug.Log("Appended packet info to previous memorystream!", debugBlockID_ProcessStream);
                }
                // Add other memory streams
                for (; appendIndex < appendList.Count; appendIndex++)
                {
                    msList.Add(appendList[appendIndex]);
                    textureCount++;
                    MultiThreadDebug.Log("Appending new texture packet memory streams to memory stream list. textureIndex now = " + textureCount, debugBlockID_ProcessStream);
                }

                if(numBytesReceived < bufferLength
                    && packetInitiated == false)
                {
                    // If you've gotten a partially empty receive and you've finished reading in a texture packet, don't wait on another receive or you'll block and freeze the program
                    break;
                }
                MultiThreadDebug.Log("Finished processing buffer for texture packet. Looping...", debugBlockID_ProcessStream);
            }

            return msList;
        }

        /// <summary>
        /// List will be null if nothing was read in. Or could be a list of multiple memory streams for multiple packets
        /// </summary>
        /// <returns></returns>
        public static List<MemoryStream> ProcessTexturePacket(ref bool packetInitiated, ref int bytesRemainingForPacket, int numBytesAvailable, byte[] buffer)
        {
            MultiThreadDebug.Log("Processing packet...\tpacketInitiated = " + packetInitiated + "; bytesRemainingForPacket = " + bytesRemainingForPacket + "; numByteAvailable = " + numBytesAvailable, debugBlockID_ProcessBuffer);

            List<MemoryStream> msList = new List<MemoryStream>();
            MemoryStream ms = new MemoryStream();
            int byteIndex = 0;

            while (numBytesAvailable > 0)
            {
                if (!packetInitiated)
                {
                    int headerSize = BitConverter.ToInt32(buffer, byteIndex);
                    byte[] headerBytes = new byte[headerSize];
                    Array.Copy(buffer, byteIndex, headerBytes, 0, headerSize);

                    int headerSizeRead;
                    int nameSize;
                    int width;
                    int height;
                    int contentSize;
                    TexturePacket.ReadHeader(headerBytes, out headerSizeRead, out nameSize, out width, out height, out contentSize);

                    //bytesRemainingForPacket = BitConverter.ToInt32(buffer, byteIndex);
                    bytesRemainingForPacket = headerSizeRead + nameSize + contentSize;
                    // Don't increment the byteIndex, because it still needs to 
                    // get processed by TexturePacket class, so we don't want 
                    // to pass over the headerSize int
                    ms = new MemoryStream();
                    packetInitiated = !packetInitiated;

                    MultiThreadDebug.Log("New texture packet started...(bytesRemainingForPacket = " + bytesRemainingForPacket + ")...", debugBlockID_ProcessBuffer);
                }
                // Read the bytes
                if (bytesRemainingForPacket > numBytesAvailable)
                {
                    MultiThreadDebug.Log("Reading all bytes in buffer...bytesRemainingForPacket = " + bytesRemainingForPacket + "...numBytesAvailable = " + numBytesAvailable, debugBlockID_ProcessBuffer);

                    ms.Write(buffer, byteIndex, numBytesAvailable);
                    msList.Add(ms);
                    byteIndex += numBytesAvailable;
                    bytesRemainingForPacket -= numBytesAvailable;
                    numBytesAvailable -= numBytesAvailable;
                    
                    break;
                }
                else if (bytesRemainingForPacket == numBytesAvailable)
                {
                    MultiThreadDebug.Log("Reading all bytes in buffer...bytesRemainingForPacket = " + bytesRemainingForPacket + "...numBytesAvailable = " + numBytesAvailable, debugBlockID_ProcessBuffer);

                    ms.Write(buffer, byteIndex, numBytesAvailable);
                    msList.Add(ms);
                    byteIndex += numBytesAvailable;
                    bytesRemainingForPacket -= numBytesAvailable;
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
                    bytesRemainingForPacket -= bytesRemainingForPacket;
                    packetInitiated = false;
                    // repeat

                    MultiThreadDebug.Log("Finishing packet...bytesRemainingForPacket = " + bytesRemainingForPacket + " (should be 0)", debugBlockID_ProcessBuffer);
                }
                ServerFinder.displayMsg = "numBytesAvailable = " + numBytesAvailable.ToString();

                MultiThreadDebug.Log("Looping in ProcessTexturePacket...(numBytesAvailable = " + numBytesAvailable + ")...(bytesRemainingForPacket = " + bytesRemainingForPacket + ")...(byteIndex = " + byteIndex + ")...(packetInitiated? = " + packetInitiated + ")", debugBlockID_ProcessBuffer);

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