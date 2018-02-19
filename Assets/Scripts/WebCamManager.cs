using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamManager : MonoBehaviour {
    public WebCamTexture VideoFeed;
    public WebCamDevice[] WebCams;
    public int CamIndex = 0;
    private int storedCamIndex;

    public GameObject Portal;

	// Use this for initialization
	void Awake () {
        WebCams = WebCamTexture.devices;
        VideoFeed = null;
        storedCamIndex = CamIndex;

        InitCamFeed(CamIndex);
	}

    void FixedUpdate()
    {
        WebCams = WebCamTexture.devices;
        if(VideoFeed == null
            && WebCams.Length > 0)
        {
            InitCamFeed(CamIndex);
        }
        else if(WebCams.Length == 0)
        {
            // Have backup image displayed for no feed
        }
        else if(VideoFeed != null)
        {
            WebCams = WebCamTexture.devices;
            if(CamIndex >= WebCams.Length)
            {
                CamIndex = storedCamIndex;
            }
            else
            {
                //SwitchFeedCamera(WebCams[CamIndex]);
            }
        }
    }
    
    private void InitCamFeed(int index)
    {
        Debug.Log("Initializing camera feed");
        if (index >= 0)
        {
            if (WebCams.Length > index)
            {

                SwitchFeedCamera(WebCams[index]);
            }
        }
    }

    public WebCamTexture SwitchFeedCamera(WebCamDevice webCam)
    {
        if(VideoFeed != null
            && VideoFeed.isPlaying)
        {
            VideoFeed.Stop();
        }
        VideoFeed = new WebCamTexture(webCam.name);
        VideoFeed.name = VideoFeed.deviceName;
        VideoFeed.Play();
        SetTexture();
        return VideoFeed;
    }

    public void SetTexture()
    {
        if(Portal != null
            && VideoFeed != null)
        {
            MeshRenderer mr = Portal.GetComponent<MeshRenderer>();
            if(mr != null)
            {
                Material m = mr.material;
                if(m != null)
                {
                    m.SetTexture("_MainTex", VideoFeed);
                }
            }
        }
    }

    public void TurnOffCamera()
    {
        if(VideoFeed != null)
        {
            VideoFeed.Stop();
        }
    }
}
