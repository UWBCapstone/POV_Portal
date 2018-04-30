using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPortal
{
    public class Movement : MonoBehaviour
    {
        static Vector3 UpDelta;
        static Vector3 LeftDelta;
        static Vector3 DownDelta;
        static Vector3 RightDelta;
        public float MovementDelta = 0.3f;
        static float Delta = 0.3f;

        static Movement()
        {
            Movement.SetDeltas();
        }

        static void SetDeltas()
        {
            UpDelta = Vector3.forward * Delta;
            LeftDelta = Vector3.left * Delta;
            DownDelta = Vector3.back * Delta;
            RightDelta = Vector3.right * Delta;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                gameObject.transform.Translate(Movement.UpDelta);
            }
            if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.Translate(Movement.LeftDelta);
            }
            if (Input.GetKey(KeyCode.S))
            {
                if(gameObject.transform.position.z <= 0)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
                }
                else
                {
                    gameObject.transform.Translate(Movement.DownDelta);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.Translate(Movement.RightDelta);
            }

            Movement.Delta = MovementDelta;
            SetDeltas();
        }
    }
}