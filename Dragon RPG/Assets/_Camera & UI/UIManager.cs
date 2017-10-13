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
        private PlayerMovement playerMovement;
        // Use this for initialization
        void Awake ()
        {
            text = GetComponentInChildren<Text>();
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerMovement.ControlModeDelegate += onControlModeChange;
        }

        // Update is called once per frame
        void onControlModeChange (string controlMode)
        {
            text.text = controlMode;
        }
    }
}