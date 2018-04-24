using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    /// <summary>
    /// Meant to reside on the PC server side
    /// </summary>
    public class WebCamManager : MonoBehaviour
    {
        public WebCamTexture[] VideoFeeds;
        public WebCamDevice[] WebCams;

        void Awake()
        {
            WebCams = WebCamTexture.devices;
            if (WebCams.Length > 0)
            {
                VideoFeeds = new WebCamTexture[WebCams.Length];
            }
            else
            {
                VideoFeeds = null;
            }

            InitCamFeeds();
        }

        void FixedUpdate()
        {
            WebCamDevice[] NewWebCamsList = WebCamTexture.devices;

            if (VideoFeeds == null
                && NewWebCamsList.Length > 0)
            {
                WebCams = NewWebCamsList;
                InitCamFeeds();
            }
            else if (NewWebCamsList.Length == 0)
            {
                // Have backup image displayed for no feed
                WebCams = NewWebCamsList;
                VideoFeeds = null;
            }
            else if (VideoFeeds != null)
            {
                if (NewWebCamsList.Length != WebCams.Length)
                {
                    TurnOffCameras();
                    WebCams = NewWebCamsList;
                    VideoFeeds = new WebCamTexture[WebCams.Length];
                    InitCamFeeds();
                }
            }
        }

        private void InitCamFeeds()
        {
            if (WebCams != null)
            {
                for (int i = 0; i < WebCams.Length; i++)
                {
                    InitCamFeed(i);
                }
            }
            else
            {
                Debug.LogError("No web cameras detected for initializing texture feeds.");
            }
        }

        private void InitCamFeed(int index)
        {
            Debug.Log("Initializing camera feed (Camera " + index + ")");
            if (WebCams != null
                && WebCams.Length > index)
            {
                UpdateCameraVideoFeed(index);
            }
        }

        public WebCamTexture UpdateCameraVideoFeed(int camIndex)
        {
            if (VideoFeeds != null
                && VideoFeeds.Length > camIndex)
            {
                if (VideoFeeds[camIndex] != null
                    && VideoFeeds[camIndex].isPlaying)
                {
                    VideoFeeds[camIndex].Stop();
                }

                if (WebCams != null
                    && WebCams.Length > camIndex)
                {
                    VideoFeeds[camIndex] = new WebCamTexture(WebCams[camIndex].name);
                    VideoFeeds[camIndex].name = GetVideoFeedName(camIndex);
                    VideoFeeds[camIndex].Play();
                    //SetTexture();
                    return VideoFeeds[camIndex];
                }
            }

            return null;
        }

        public string GetVideoFeedName(int camIndex)
        {
            if (VideoFeeds != null
                && VideoFeeds.Length > camIndex)
            {
                if (VideoFeeds[camIndex] != null)
                {
                    return VideoFeeds[camIndex].deviceName + "_(" + camIndex.ToString() + ")";
                }
            }

            // else
            return string.Empty;
        }
        
        public void TurnOffCameras()
        {
            if (VideoFeeds != null)
            {
                foreach (WebCamTexture videoFeed in VideoFeeds)
                {
                    videoFeed.Stop();
                }
            }
        }

        public void TurnOffCamera(int camIndex)
        {
            if (VideoFeeds != null
                && VideoFeeds.Length > camIndex)
            {
                VideoFeeds[camIndex].Stop();
            }
        }

        public void GetNumWebcams()
        {

        }

        public int NumVideoFeeds
        {
            get
            {
                if (VideoFeeds == null)
                {
                    return 0;
                }
                else
                {
                    return VideoFeeds.Length;
                }
            }
        }

        public int NumWebCams
        {
            get
            {
                if (WebCams == null)
                {
                    return 0;
                }
                else
                {
                    return WebCams.Length;
                }
            }
        }

        /// <summary>
        /// Returns a boolean array stating which cameras are playing / active.
        /// </summary>
        public bool[] PlayingCameras
        {
            get
            {
                bool[] playingCameras = new bool[VideoFeeds.Length];
                for (int i = 0; i < VideoFeeds.Length; i++)
                {
                    if (VideoFeeds[i] != null
                        && VideoFeeds[i].isPlaying)
                    {
                        playingCameras[i] = true;
                    }
                    else
                    {
                        playingCameras[i] = false;
                    }
                }

                return playingCameras;
            }
        }

        public WebCamTexture GetTextureFor(WebcamDeviceNames deviceName)
        {
            for(int i = 0; i < NumWebCams; i++)
            {
                if(VideoFeeds.Length > i)
                {
                    var specsName = WebCamSpecsManager.WebCamDeviceToSpecsName(WebCams[i]);
                    if(specsName == deviceName)
                    {
                        return VideoFeeds[i];
                    }
                }
                else
                {
                    Debug.LogError("Inconsistent number of webcams and feeds.");
                    break;
                }
            }

            return null;
        }
    }
}