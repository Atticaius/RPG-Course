using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    public class CameraFollow : MonoBehaviour
    {

        [SerializeField] float cameraTurnSpeed = 10f;
        private GameObject player;

        // Use this for initialization
        void Start ()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        void LateUpdate ()
        {
            transform.position = player.transform.position;

            if (Input.GetAxis("Arrow Horizontal") != 0)
            {
                transform.RotateAround(player.transform.position, Vector3.up, Input.GetAxis("Arrow Horizontal") * cameraTurnSpeed);
            }
            if (Input.GetAxis("Horizontal") != 0)
            {
                transform.RotateAround(player.transform.position, Vector3.up, Input.GetAxis("Horizontal") * cameraTurnSpeed);
            }
            if (Input.GetAxis("Arrow Vertical") != 0)
            {
                transform.RotateAround(player.transform.position, Vector3.right, Input.GetAxis("Arrow Vertical"));
            }
        }
    }
}
