using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace ARPortal
{
    public static class MultiThreadDebug
    {
        public static bool display = true;
        public static Semaphore _pool;
        public static List<string> messagePool;
        public static List<int> blockedIDs;

        static MultiThreadDebug()
        {
            _pool = new Semaphore(5, 5);
            messagePool = new List<string>();
            blockedIDs = new List<int>();
            BlockIDs();
        }

        public static void BlockIDs()
        {
            List<int> blockIDs = new List<int>();
            blockIDs.Add(Socket_Base_PC.debugBlockID_ProcessBuffer);
            blockIDs.Add(Socket_Base_PC.debugBlockID_ProcessStream);
            blockedIDs.AddRange(blockIDs);
        }

        public static void Log(string msg)
        {
            _pool.WaitOne();
            messagePool.Add(msg);
            _pool.Release(1);
        }

        public static void Log(string msg, int blockID)
        {
            if (!blockedIDs.Contains(blockID))
            {
                Log(msg);
            }
        }

        public static void Flush()
        {
            if (display)
            {
                List<string> messagesToFlush = new List<string>(messagePool);
                messagePool = new List<string>();
                foreach (string msg in messagesToFlush)
                {
                    Debug.Log("THREAD: " + msg);
                }
            }
        }
    }
}