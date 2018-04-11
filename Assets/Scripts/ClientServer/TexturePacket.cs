using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace ARPortal
{
    public class TexturePacket
    {
        public const int WEBCAM_DEVICENAME_SIZE = 20;
        public const int TEXTURE_SIZE = 307200; // 640 X 480 (pixels)
        public const int BYTES_PER_CHANNEL = 4;

        private const int NUM_CHANNELS = 4;

        public byte[] textureBytes;
        public int width;
        public int height;
        public byte[] headerBytes;
        public byte[] nameBytes;
        public byte[] bytes;

        public TexturePacket(Texture2D tex)
        {
            width = tex.width;
            height = tex.height;

            Color[] pixels = tex.GetPixels();
            textureBytes = WriteContent(pixels);
            headerBytes = WriteHeader(tex);
            nameBytes = WriteNameBytes(tex.name);

            bytes = CompileByteArrays(headerBytes, nameBytes, textureBytes);
        }

        public TexturePacket(WebCamTexture feed)
        {
            width = feed.width;
            height = feed.height;

            Color[] pixels = feed.GetPixels();
            textureBytes = WriteContent(pixels);
            headerBytes = WriteHeader(feed);
            nameBytes = WriteNameBytes(feed.name);

            bytes = CompileByteArrays(headerBytes, nameBytes, textureBytes);
        }

        public TexturePacket(int width, int height, Color[] pixels, string name)
        {
            this.width = width;
            this.height = height;

            textureBytes = WriteContent(pixels);
            headerBytes = WriteHeader(width, height, name);
            nameBytes = WriteNameBytes(name);

            bytes = CompileByteArrays(headerBytes, nameBytes, textureBytes);
        }

        public static Texture2D ReadFromBytes(byte[] packetBytes)
        {
            MultiThreadDebug.Log("Packet bytes size = " + packetBytes.Length);

            int headerSize;
            int nameSize;
            int width;
            int height;
            int contentSize;
            int byteIndex = ReadHeader(packetBytes, out headerSize, out nameSize, out width, out height, out contentSize);

            MultiThreadDebug.Log("Read header: headerSize " + headerSize + "; nameSize: " + nameSize + "; width: " + width + "; height: " + height + "; contentSize: " + contentSize);

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            string name = ReadName(packetBytes, byteIndex, nameSize);
            byteIndex += nameSize;
            tex.name = name;

            MultiThreadDebug.Log("Read name = " + name);

            tex.SetPixels(ReadContent(packetBytes, byteIndex, contentSize));
            tex.Apply();

            return tex;
        }

        public byte[] WriteHeader(Texture2D tex)
        {
            // Size of name of texture
            // Width
            // Height
            // Size of content
            
            //int nameSize = tex.name.Length;
            int nameSize = StringToBytes(tex.name).Length;
            int width = tex.width;
            int height = tex.height;
            int contentSize = textureBytes.Length;

            return writeHeader(nameSize, width, height, contentSize);
        }

        public byte[] WriteHeader(WebCamTexture feed)
        {
            // Size of name of texture
            // Width
            // Height
            // Size of content
            
            //int nameSize = feed.name.Length;
            int nameSize = StringToBytes(feed.name).Length;
            int width = feed.width;
            int height = feed.height;
            int contentSize = textureBytes.Length;

            return writeHeader(nameSize, width, height, contentSize);
        }

        public byte[] WriteHeader(int width, int height, string name)
        {
            int contentSize = textureBytes.Length;
            int nameSize = StringToBytes(name).Length;

            return writeHeader(nameSize, width, height, contentSize);
        }

        private byte[] writeHeader(int nameSize, int width, int height, int contentSize)
        {
            //byte[] headerSizeBytes = BitConverter.GetBytes(headerSize);
            byte[] nameSizeBytes = BitConverter.GetBytes(nameSize);
            byte[] widthBytes = BitConverter.GetBytes(width);
            byte[] heightBytes = BitConverter.GetBytes(height);
            byte[] contentSizeBytes = BitConverter.GetBytes(contentSize);
            
            int totalHeaderSize = nameSizeBytes.Length + widthBytes.Length + heightBytes.Length + contentSizeBytes.Length;
            totalHeaderSize += BitConverter.GetBytes(totalHeaderSize).Length;
            byte[] headerSizeBytes = BitConverter.GetBytes(totalHeaderSize);

            MemoryStream ms = new MemoryStream();
            ms.Write(headerSizeBytes, 0, headerSizeBytes.Length); // headerSize
            ms.Write(nameSizeBytes, 0, nameSizeBytes.Length); // nameSize
            ms.Write(widthBytes, 0, widthBytes.Length); // width
            ms.Write(heightBytes, 0, heightBytes.Length); // height
            ms.Write(contentSizeBytes, 0, contentSizeBytes.Length); // contentSize

            byte[] headerBytes = ms.ToArray();
            return headerBytes;
        }

        // Returns the index at which to start reading the content bytes
        public static int ReadHeader(byte[] headerBytes, out int headerSize, out int nameSize, out int width, out int height, out int contentSize)
        {
            // Size of header
            // Size of name of texture
            // Width
            // Height
            // Size of content

            int index = 0;
            int increment = BitConverter.GetBytes(15).Length;

            headerSize = BitConverter.ToInt32(headerBytes, index);
            index += increment;
            nameSize = BitConverter.ToInt32(headerBytes, index);
            index += increment;
            width = BitConverter.ToInt32(headerBytes, index);
            index += increment;
            height = BitConverter.ToInt32(headerBytes, index);
            index += increment;
            contentSize = BitConverter.ToInt32(headerBytes, index);
            index += increment;

            return index;
        }

        public byte[] WriteNameBytes(string name)
        {
            return StringToBytes(name);
        }

        public static string ReadName(byte[] packetBytes, int startIndex, int nameSize)
        {
            byte[] readNameBytes = new byte[nameSize];
            Array.Copy(packetBytes, startIndex, readNameBytes, 0, nameSize);
            //Array.Copy(packetBytes, 0, readNameBytes, nameSize); // Assumes nameBytes comes before content bytes
            return ReadNameBytes(readNameBytes);
        }

        public static string ReadNameBytes(byte[] nameBytes)
        {
            return BytesToString(nameBytes);
        }

        public byte[] WriteContent(Color[] pixels)
        {
            byte[] texBytes = new byte[pixels.Length * NUM_CHANNELS * BYTES_PER_CHANNEL]; // r g b a
            for (int pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex++)
            {
                Color pixel = pixels[pixelIndex];
                for (int channelIndex = 0; channelIndex < NUM_CHANNELS; channelIndex++)
                {
                    int texBytesIndex = (pixelIndex * NUM_CHANNELS * BYTES_PER_CHANNEL) + (channelIndex * BYTES_PER_CHANNEL);
                    byte[] channelBytes = BitConverter.GetBytes((Int32)(pixel[channelIndex]*255));
                    Array.Copy(channelBytes, 0, texBytes, texBytesIndex, channelBytes.Length);

                    //for(int channelByteIndex = 0; channelByteIndex < BYTES_PER_CHANNEL; channelByteIndex++)
                    //{
                    //    int texBytesIndex = (pixelIndex * NUM_CHANNELS * BYTES_PER_CHANNEL) + (channelIndex * BYTES_PER_CHANNEL) + channelByteIndex;

                    //    texBytes[texBytesIndex] = BitConverter(pixel[channelIndex])
                    //}
                    //texBytes[i * NUM_CHANNELS + j] = (byte)(pixel[j] * 255);
                }
            }

            if (texBytes.Length > TEXTURE_SIZE * BYTES_PER_CHANNEL * NUM_CHANNELS)
            {
                Debug.LogError("Inadequate packet size allocated for textures. Allocated: (" + (TEXTURE_SIZE * BYTES_PER_CHANNEL * NUM_CHANNELS) + "); Needed: (" + texBytes.Length + ")");
            }

            return texBytes;
        }

        public static Color[] ReadContent(byte[] packetBytes, int startIndex, int contentSize)
        {
            int totalPixels = contentSize / (NUM_CHANNELS * BYTES_PER_CHANNEL);
            Color[] colors = new Color[totalPixels];

            for (int pixelIndex = 0; pixelIndex < totalPixels; pixelIndex++)
            {
                Color c = new Color();
                for (int channelIndex = 0; channelIndex < NUM_CHANNELS; channelIndex++)
                {
                    byte[] bytesForChannel = new byte[BYTES_PER_CHANNEL];
                    int copyStartIndex = startIndex + pixelIndex * NUM_CHANNELS * BYTES_PER_CHANNEL + channelIndex * BYTES_PER_CHANNEL;
                    Array.Copy(packetBytes, copyStartIndex, bytesForChannel, 0, BYTES_PER_CHANNEL);
                    c[channelIndex] = (float)(BitConverter.ToInt32(bytesForChannel, 0)/255.0f);
                }
                colors[pixelIndex] = c;
            }

            //int pixelsRead = 0;
            //while(pixelsRead < totalPixels)
            //{
            //    for(int i = 0; i < NUM_CHANNELS; i++)
            //    {
            //        for(int j = 0; j < BYTES_PER_CHANNEL; j++)
            //        {
            //            colors[i][j] = (float)(packetBytes[startIndex + i * BYTES_PER_CHANNEL + j]);
            //        }
            //    }
            //    pixelsRead++;
            //}

            return colors;
        }

        private byte[] CompileByteArrays(byte[] headerBytes, byte[] nameBytes, byte[] textureBytes)
        {
            byte[] compiledArray = new byte[headerBytes.Length + nameBytes.Length + textureBytes.Length];
            Array.Copy(headerBytes, compiledArray, headerBytes.Length);
            Array.Copy(nameBytes, 0, compiledArray, headerBytes.Length, nameBytes.Length);
            Array.Copy(textureBytes, 0, compiledArray, headerBytes.Length + nameBytes.Length, textureBytes.Length);
            //Array.Copy(nameBytes, compiledArray, nameBytes.Length);
            //Array.Copy(textureBytes, compiledArray, textureBytes.Length);

            return compiledArray;
        }

        public static string BytesToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static byte[] StringToBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}