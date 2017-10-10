﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] Text text;
    private PlayerMovement playerMovement;
	// Use this for initialization
	void Start () {
        text = GetComponentInChildren<Text>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.ControlModeDelegate += onControlModeChange;
	}
	
	// Update is called once per frame
	void onControlModeChange (string controlMode) {
        text.text = controlMode;
	}
}
