﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundCalc : MonoBehaviour {
    public Vector3 MyExtents;

    public void Awake()
    {
        MyExtents = BoundCalc.Extents(gameObject);
    }

    public void Update()
    {
        MyExtents = BoundCalc.Extents(gameObject);
    }

    public static Vector3 Corner00(GameObject go)
    {
        Vector3 cent = Center(go);
        Vector3 extents = Extents(go);
        Vector3 P00 = 
            new Vector3(
                cent.x - extents.x, 
                cent.y - extents.y, 
                cent.z);
        return P00;
    }
    
    public static Vector3 Corner01(GameObject go)
    {
        Vector3 cent = Center(go);
        Vector3 extents = Extents(go);
        Vector3 P00 =
            new Vector3(
                cent.x - extents.x,
                cent.y + extents.y,
                cent.z);
        return P00;
    }
    
    public static Vector3 Corner10(GameObject go)
    {
        Vector3 cent = Center(go);
        Vector3 extents = Extents(go);
        Vector3 P00 =
            new Vector3(
                cent.x + extents.x,
                cent.y - extents.y,
                cent.z);
        return P00;
    }

    public static Vector3 Corner11(GameObject go)
    {
        Vector3 cent = Center(go);
        Vector3 extents = Extents(go);
        Vector3 P00 =
            new Vector3(
                cent.x + extents.x,
                cent.y + extents.y,
                cent.z);
        return P00;
    }

    public static Vector3 Center(GameObject go)
    {
        if (go != null)
        {
            if (go.GetComponent<MeshFilter>() == null)
            {
                return go.transform.position;
            }
            else
            {
                Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                Bounds bounds = mesh.bounds;
                return bounds.center + go.transform.position;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Vector3 Extents(GameObject go)
    {
        if (go != null)
        {
            if (go.GetComponent<MeshFilter>() == null)
            {
                return Vector3.one * 0.01f;
            }
            else
            {
                Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                Bounds bounds = mesh.bounds;
                Vector3 actualExtents = bounds.extents;
                actualExtents.x *= go.transform.localScale.x;
                actualExtents.y *= go.transform.localScale.y;
                actualExtents.z *= go.transform.localScale.z;
                //Debug.Log("Extents = " + actualExtents);
                return actualExtents;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }
}