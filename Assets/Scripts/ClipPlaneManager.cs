using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipPlaneManager {
    private PlaneRect clipPlane;
    private Vector3 pos;
    private Vector3 up;
    private Vector3 forward;
    private Vector3 right;
    private float deg;
    private float dis;
    private float hypotenuse;
    private float opposite;
    private float nearClipDis;

    public ClipPlaneManager(Camera cam)
    {
        pos = cam.transform.position;
        up = cam.transform.up;
        forward = cam.transform.forward;
        right = cam.transform.right;

        deg = cam.fieldOfView / 2;
        dis = cam.farClipPlane;
        hypotenuse = dis / Mathf.Cos(Mathf.Deg2Rad * deg);
        opposite = Mathf.Sin(Mathf.Deg2Rad * deg) * hypotenuse;
        nearClipDis = cam.nearClipPlane;

        //Debug.Log("Right vector is " + right);
        //Debug.Log("Up vector is " + up);
        //Debug.Log("Opposite is " + opposite);
        //Debug.Log("Degree is " + deg);
        //Debug.Log("Distance is " + dis);
        //Debug.Log("Hypotenuse is " + hypotenuse);

        clipPlane = new PlaneRect(calc00(), calc11(), -forward);
    }

    private Vector3 calc00()
    {
        return pos - right.normalized * opposite - up.normalized * opposite + forward * dis;
    }

    private Vector3 calc01()
    {
        return pos - right.normalized * opposite + up.normalized * opposite + forward * dis;
    }

    private Vector3 calc10()
    {
        return pos + right.normalized * opposite - up.normalized * opposite + forward * dis;
    }

    private Vector3 calc11()
    {
        return pos + right.normalized * opposite + up.normalized * opposite + forward * dis;
    }

    public RaycastHit Intersect(Ray ray, Vector3 origin)
    {
        return clipPlane.Intersect(ray, origin);
    }
    
    #region Properties
    public PlaneRect ClipRect
    {
        get
        {
            return clipPlane;
        }
    }
    #endregion
}
