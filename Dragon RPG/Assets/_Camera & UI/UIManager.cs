using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] Text text;
        GameObject player;

        private void Start ()
        {
            text = GetComponentInChildren<Text>();
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerMovement>().ControlModeDelegate += onControlModeChange;
        }

        // Update is called once per frame
        void onControlModeChange (string controlMode)
        {
            text.text = controlMode;
        }
    }
}