﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace ARPortal
{
    public class BodyManager : MonoBehaviour
    {
        public bool debugging = false;
        public KinectSensor sensor;
        public BodyFrameReader reader;
        //public KinectManager sourceManager;
        public Body[] FrameBodyData;
        public float BoxScale = 0.1f;
        public float BodyScaleX = 2.3f;//5f;
        public float BodyScaleY = 4.14f;//9f;
        public float BodyScaleZ = 2.3f;//5f;
        public static float BodyScaleX_s = 2.3f;//5f; // Note: Y axis scaling is weird. It has an additional multiplicative value applied.
        public static float BodyScaleY_s = 4.14f;//9f;
        public static float BodyScaleZ_s = 2.3f;//5f;
        public Camera MainCamera;
        public List<JointType> visibleJointTypes;

        //public BodyFrame bodyFrame;
        //public BodyFrameReader reader; // provides access to individual bodies through GetAndRefreshBodyData
        //public BodyFrameSource source; // bodycount (input vector for GetAndRefreshBodyData)

        public Dictionary<ulong, GameObject> BodyMap = new Dictionary<ulong, GameObject>();
        
        private Dictionary<JointType, JointType> _BoneMap = new Dictionary<JointType, JointType>()
    {
        { JointType.FootLeft, JointType.AnkleLeft },
        { JointType.AnkleLeft, JointType.KneeLeft },
        { JointType.KneeLeft, JointType.HipLeft },
        { JointType.HipLeft, JointType.SpineBase },

        { JointType.FootRight, JointType.AnkleRight },
        { JointType.AnkleRight, JointType.KneeRight },
        { JointType.KneeRight, JointType.HipRight },
        { JointType.HipRight, JointType.SpineBase },

        { JointType.HandTipLeft, JointType.HandLeft },
        { JointType.ThumbLeft, JointType.HandLeft },
        { JointType.HandLeft, JointType.WristLeft },
        { JointType.WristLeft, JointType.ElbowLeft },
        { JointType.ElbowLeft, JointType.ShoulderLeft },
        { JointType.ShoulderLeft, JointType.SpineShoulder },

        { JointType.HandTipRight, JointType.HandRight },
        { JointType.ThumbRight, JointType.HandRight },
        { JointType.HandRight, JointType.WristRight },
        { JointType.WristRight, JointType.ElbowRight },
        { JointType.ElbowRight, JointType.ShoulderRight },
        { JointType.ShoulderRight, JointType.SpineShoulder },

        { JointType.SpineBase, JointType.SpineMid },
        { JointType.SpineMid, JointType.SpineShoulder },
        { JointType.SpineShoulder, JointType.Neck },
        { JointType.Neck, JointType.Head },
    };

        // Use this for initialization
        //void Awake()
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            BodyScaleX_s = BodyScaleX; // Make it easier to set static variable from the Editor
            BodyScaleY_s = BodyScaleY;
            BodyScaleZ_s = BodyScaleZ;

            UpdateFrameBodyData();
            List<ulong> newBodyIDs = IdentifyNewBodyIDs();
            RefreshBodies(newBodyIDs);
        }
        
        public void Init()
        {
            sensor = KinectSensor.GetDefault();
            reader = sensor.BodyFrameSource.OpenReader();

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }
            
            FrameBodyData = new Body[sensor.BodyFrameSource.BodyCount];
        }

        public void UpdateFrameBodyData()
        {
            BodyFrame frame = reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.GetAndRefreshBodyData(FrameBodyData);

                frame.Dispose();
                frame = null;
            }
        }

        /// <summary>
        /// Cull offscreen body tracking for the body dictionary.
        /// </summary>
        public List<ulong> IdentifyNewBodyIDs()
        {
            List<ulong> newBodyIDs = new List<ulong>();

            List<ulong> onScreenIDs = GetOnScreenBodyIDs();
            CullOffscreenBodies(onScreenIDs);

            // Figure out which body IDs are new
            foreach(var onScreenID in onScreenIDs)
            {
                if (!BodyMap.ContainsKey(onScreenID))
                {
                    newBodyIDs.Add(onScreenID);
                }
            }

            return newBodyIDs;
        }

        public List<ulong> GetOnScreenBodyIDs()
        {
            List<ulong> onScreenIDs = new List<ulong>();
            foreach (Body body in FrameBodyData)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    if (!BodyMap.ContainsKey(body.TrackingId))
                    {
                        Debug.Log("New trackable body found that does not exist in Bodies dictionary! " + body.TrackingId);
                    }
                    onScreenIDs.Add(body.TrackingId);
                }
                else
                {
                    if (body.TrackingId != 0)
                    {
                        Debug.Log("Non tracked body found..." + body.TrackingId);
                    }
                }
            }

            return onScreenIDs;
        }

        public void CullOffscreenBodies(List<ulong> onScreenIDs)
        {
            List<ulong> knownIDs = new List<ulong>(BodyMap.Keys);

            // Cull the 
            foreach (ulong knownID in knownIDs)
            {
                if (!onScreenIDs.Contains(knownID))
                {
                    ulong offScreenID = knownID;
                    Destroy(BodyMap[offScreenID]);
                    BodyMap.Remove(offScreenID);
                }
            }
        }

        public void RefreshBodies(List<ulong> newBodyIDs)
        {
            // Refresh a body if it exists, or create a new one if needed
            //foreach(ulong bodyID in _Bodies.Keys)
            HashSet<ulong> oldOnScreenBodyIDs = new HashSet<ulong>(GetOnScreenBodyIDs());
            oldOnScreenBodyIDs.RemoveWhere(x => newBodyIDs.Contains(x));
            // ERROR TESTING - lambda may freeze Unity

            foreach(ulong bodyID in newBodyIDs)
            {
                if (!BodyMap.ContainsKey(bodyID))
                {
                    // Create a new body character
                    if (!debugging)
                    {
                        BodyMap.Add(bodyID, CreateBody(bodyID, visibleJointTypes));
                    }
                    else
                    {
                        BodyMap.Add(bodyID, CreateBody(bodyID));
                    }
                }
            }

            foreach (ulong oldBodyID in oldOnScreenBodyIDs)
            {
                // Refresh an existing body character
                Body targetBody = null;
                foreach (Body body in FrameBodyData)
                {
                    if (body.TrackingId.Equals(oldBodyID))
                    {
                        targetBody = body;
                        break;
                    }
                }
                RefreshBodyObject(targetBody, BodyMap[oldBodyID]);
            }
        }

        public GameObject CreateBody(ulong id, List<JointType> visibleJoints)
        {
            GameObject body = new GameObject("Body:" + id);

            Debug.Log("Creating new body of joint count " + visibleJoints.Count);
            for(int i = 0; i < visibleJoints.Count; i++)
            {
                Debug.Log("Adding visible joint object: " + visibleJoints[i].ToString());

                JointType jt = visibleJoints[i];
                GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                jointObj.transform.localScale = Vector3.one * BoxScale; //new Vector3(0.3f, 0.3f, 0.3f);
                jointObj.name = jt.ToString();
                jointObj.transform.parent = body.transform;
            }

            return body;
        }

        public GameObject CreateBody(ulong id)
        {
            List<JointType> allJointTypes = new List<JointType>();
            foreach(JointType jt in System.Enum.GetValues(typeof(JointType)))
            {
                allJointTypes.Add(jt);
            }
            return CreateBody(id, allJointTypes);
        }

        public void RefreshBodyObject(Body body, GameObject bodyObject)
        {
            Windows.Kinect.Joint spineJoint = body.Joints[JointType.SpineBase];
            Vector3 spineBasePos = GetVector3FromJoint(spineJoint);
            Vector3 spineBaseOrig = new Vector3(spineJoint.Position.X, spineJoint.Position.Y, spineJoint.Position.Z);

            for (JointType jt = JointType.SpineBase; jt < JointType.ThumbRight; jt++)
            {
                Windows.Kinect.Joint sourceJoint = body.Joints[jt];
                Windows.Kinect.Joint? targetJoint = null;

                if (_BoneMap.ContainsKey(jt))
                {
                    targetJoint = body.Joints[_BoneMap[jt]];
                }

                Transform jointObj = bodyObject.transform.Find(jt.ToString());
                if (jointObj != null)
                {
                    jointObj.localPosition = GetVector3FromJoint(sourceJoint);
                }

                //int ZClamp = 0;
                //Vector3 clampedPos = GetZClampedVector3FromJoint(sourceJoint, ZClamp, spineBasePos);
                //clampedPos = ScaleXY(clampedPos, sourceJoint, spineBaseOrig);
                //if (jointObj != null)
                //{
                //    jointObj.localPosition = clampedPos;
                //}
            }
        }

        public static Color GetStateColor(TrackingState state)
        {
            switch (state)
            {
                case TrackingState.Tracked:
                    return Color.green;
                case TrackingState.Inferred:
                    return Color.red;
                default:
                    return Color.black;
            }
        }

        private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
        {
            // Only used to grab the spine base for comparison
            //float scale = 10;
            //return new Vector3(joint.Position.X * BodyScaleX_s, joint.Position.Y * BodyScaleY_s, joint.Position.Z * BodyScaleZ_s);
            return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
        }

        private static Vector3 GetZClampedVector3FromJoint(Windows.Kinect.Joint joint, int ZClamp, Vector3 spineBasePos)
        {
            //float scale = 10;
            //return new Vector3(joint.Position.X * BodyScaleX_s, joint.Position.Y * BodyScaleY_s, (joint.Position.Z * BodyScaleZ_s - spineBasePos.z) + ZClamp);
            return new Vector3(joint.Position.X * BodyScaleX_s, joint.Position.Y * BodyScaleY_s, ZClamp);
        }

        private static Vector3 ScaleXY(Vector3 orig, Windows.Kinect.Joint joint, Vector3 jointSpinePos)
        {
            float opp1X = joint.Position.X - jointSpinePos.x;
            float adj2 = orig.z;
            float adj1 = joint.Position.Z;

            float opp1Y = joint.Position.Y - jointSpinePos.y;

            float scaleX = opp1X * adj2 / adj1;
            float scaleY = opp1Y * adj2 / adj1;

            //if (joint.JointType.Equals(JointType.WristRight))
            //{
            //    Debug.Log("Scaling for joint " + joint.JointType.ToString() + "...");
            //    Debug.Log("\tXScale = " + scaleX);
            //    Debug.Log("\tYScale = " + scaleY);
            //    Debug.Log("\tOrig X = " + orig.x);
            //    Debug.Log("\tOrig Y = " + orig.y);
            //    Debug.Log("\tjoint Pos.z = " + adj1);
            //    Debug.Log("\tadj2 = " + adj2);
            //}
            
            return orig + new Vector3(Mathf.Abs(orig.x * scaleX), Mathf.Abs(orig.y * scaleY), 0);
        }

        public void CloseSensorAndReader()
        {
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (sensor != null)
            {
                if (sensor.IsOpen)
                {
                    sensor.Close();
                }

                sensor = null;
            }
        }

        public void OnApplicationQuit()
        {
            CloseSensorAndReader();
        }

        public Windows.Kinect.Joint GetJoint(JointType jointType)
        {
            Windows.Kinect.Joint j = new Windows.Kinect.Joint();
            j.TrackingState = TrackingState.NotTracked;

            foreach(Body body in FrameBodyData)
            {
                if (body != null
                    && body.IsTracked)
                {
                    j = body.Joints[jointType];
                }
            }

            return j;
        }

        public static bool isValidJoint(Windows.Kinect.Joint joint)
        {
            if (joint.TrackingState.Equals(TrackingState.NotTracked))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetVisibleJoints(List<JointType> visibleJointTypesList)
        {
            if(visibleJointTypesList == null)
            {
                visibleJointTypes = new List<JointType>();
            }
            else
            {
                visibleJointTypes = new List<JointType>(visibleJointTypesList);
            }
        }

        public GameObject GetJointObject(JointType jointType)
        {
            GameObject jointObj = GameObject.Find(jointType.ToString());
            return jointObj;
        }
    }
}