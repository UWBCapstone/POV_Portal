using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class ShaderSetter : MonoBehaviour
    {
        public Vector2 LowerLeft;
        public Vector2 UpperLeft;
        public Vector2 UpperRight;
        public Vector2 LowerRight;
        public Texture2D Texture;

        public void Awake()
        {
            ////UVCalc c = Viewer.GetComponent<UVCalc>();
            //UVCalc c = gameObject.GetComponent<UVCalc>();
            //LowerLeft = c.LowerLeft;
            //UpperLeft = c.UpperLeft;
            //UpperRight = c.UpperRight;
            //LowerRight = c.LowerRight;
        }

        public void Update()
        {
            //UVCalc c = Viewer.GetComponent<UVCalc>();
            UVCalc c = gameObject.GetComponent<UVCalc>();
            LowerLeft = c.LowerLeft;
            UpperLeft = c.UpperLeft;
            UpperRight = c.UpperRight;
            LowerRight = c.LowerRight;

            Material mat = gameObject.GetComponent<Renderer>().material;
            mat.SetVector("_uvLL", LowerLeft);
            mat.SetVector("_uvUL", UpperLeft);
            mat.SetVector("_uvLR", LowerRight);
            mat.SetTexture("_MainTex", Texture);
            //mat.SetVector("_uvLL", new Vector4(0, 0, 0, 0));
            //mat.SetVector("_uvUL", new Vector4(0, 1, 0, 0));
            //mat.SetVector("_uvLR", new Vector4(1, 0, 0, 0));

            //mat.mainTexture.mipMapBias = -100000000.0f;
        }
    }
}