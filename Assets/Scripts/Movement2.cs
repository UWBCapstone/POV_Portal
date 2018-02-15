using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2 : MonoBehaviour
{
    static Vector3 UpDelta;
    static Vector3 LeftDelta;
    static Vector3 DownDelta;
    static Vector3 RightDelta;
    static float Delta = 0.3f;

    static Movement2()
    {
        UpDelta = Vector3.forward * Delta;
        LeftDelta = Vector3.left * Delta;
        DownDelta = Vector3.back * Delta;
        RightDelta = Vector3.right * Delta;
    }

    void Awake()
    {
        Movement2.Delta = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.Translate(Movement2.UpDelta);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.Translate(Movement2.LeftDelta);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.Translate(Movement2.DownDelta);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.Translate(Movement2.RightDelta);
        }
    }
}
