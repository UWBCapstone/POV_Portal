using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour {
    public enum Orientation
    {
        TL,
        TM,
        TR,
        ML,
        MM,
        MR,
        BL,
        BM,
        BR
    }

    public string SpotSelected;
    private Orientation spotSelected;

    //private Dropdown TopLeft;
    //private Dropdown TopMiddle;
    //private Dropdown TopRight;
    //private Dropdown MiddleLeft;
    //private Dropdown Middle;
    //private Dropdown MiddleRight;
    //private Dropdown BottomLeft;
    //private Dropdown BottomMiddle;
    //private Dropdown BottomRight;

    //private GameObject Cam1_TL;
    //private GameObject Cam1_TM;
    //private GameObject Cam1_TR;
    //private GameObject Cam1_ML;
    //private GameObject Cam1_MM;
    //private GameObject Cam1_MR;
    //private GameObject Cam1_BL;
    //private GameObject Cam1_BM;
    //private GameObject Cam1_BR;

    //private GameObject Cam2_TL;
    //private GameObject Cam2_TM;
    //private GameObject Cam2_TR;
    //private GameObject Cam2_ML;
    //private GameObject Cam2_MM;
    //private GameObject Cam2_MR;
    //private GameObject Cam2_BL;
    //private GameObject Cam2_BM;
    //private GameObject Cam2_BR;

    // Use this for initialization
    void Awake () {
        List<string> options = new List<string>();
        options.Add("First");
        options.Add("Second");

        //TopLeft.AddOptions(options);
        //TopMiddle.AddOptions(options);
        //TopRight.AddOptions(options);
        //MiddleLeft.AddOptions(options);
        //Middle.AddOptions(options);
        //MiddleRight.AddOptions(options);
        //BottomLeft.AddOptions(options);
        //BottomMiddle.AddOptions(options);
        //BottomRight.AddOptions(options);

        SpotSelected = "BL";
        spotSelected = Orientation.BL;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.N))
        {
            switch (spotSelected)
            {
                case Orientation.BL:
                    spotSelected = Orientation.BM;
                    break;
                case Orientation.BM:
                    spotSelected = Orientation.BR;
                    break;
                case Orientation.BR:
                    spotSelected = Orientation.ML;
                    break;
                case Orientation.ML:
                    spotSelected = Orientation.MM;
                    break;
                case Orientation.MM:
                    spotSelected = Orientation.MR;
                    break;
                case Orientation.MR:
                    spotSelected = Orientation.TL;
                    break;
                case Orientation.TL:
                    spotSelected = Orientation.TM;
                    break;
                case Orientation.TM:
                    spotSelected = Orientation.TR;
                    break;
                case Orientation.TR:
                    spotSelected = Orientation.BL;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            spotSelected = Orientation.BL;
            SpotSelected = "BL";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            spotSelected = Orientation.BM;
            SpotSelected = "BM";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            spotSelected = Orientation.BR;
            SpotSelected = "BR";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            spotSelected = Orientation.ML;
            SpotSelected = "ML";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            spotSelected = Orientation.MM;
            SpotSelected = "MM";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            spotSelected = Orientation.MR;
            SpotSelected = "MR";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            spotSelected = Orientation.TL;
            SpotSelected = "TL";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            spotSelected = Orientation.TM;
            SpotSelected = "TM";
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            spotSelected = Orientation.TR;
            SpotSelected = "TR";
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCameraScene(spotSelected, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCameraScene(spotSelected, 1);
        }
	}

    private void SetCameraScene(Orientation spot, int sceneNum)
    {
        Camera cam1 = GetCameraObject(spot, 0).GetComponent<Camera>();
        Camera cam2 = GetCameraObject(spot, 1).GetComponent<Camera>();

        if(sceneNum == 0)
        {
            cam1.depth = -1;
            cam2.depth = -2;
        }
        else
        {
            cam1.depth = -2;
            cam2.depth = -1;
        }
    }
    
    private GameObject GetCameraObject(Orientation spot, int sceneNum)
    {
        GameObject camObj = null;

        switch (spot)
        {
            case Orientation.TL:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_TL");
                else
                    camObj = GameObject.Find("Cam2_TL");
                break;
            case Orientation.TM:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_TM");
                else
                    camObj = GameObject.Find("Cam2_TM");
                break;
            case Orientation.TR:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_TR");
                else
                    camObj = GameObject.Find("Cam2_TR");
                break;
            case Orientation.ML:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_ML");
                else
                    camObj = GameObject.Find("Cam2_ML");
                break;
            case Orientation.MM:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_MM");
                else
                    camObj = GameObject.Find("Cam2_MM");
                break;
            case Orientation.MR:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_MR");
                else
                    camObj = GameObject.Find("Cam2_MR");
                break;
            case Orientation.BL:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_BL");
                else
                    camObj = GameObject.Find("Cam2_BL");
                break;
            case Orientation.BM:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_BM");
                else
                    camObj = GameObject.Find("Cam2_BM");
                break;
            case Orientation.BR:
                if (sceneNum == 0)
                    camObj = GameObject.Find("Cam_BR");
                else
                    camObj = GameObject.Find("Cam2_BR");
                break;
        }

        return camObj;
    }
}
