﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class EnemyHealthBar : MonoBehaviour
    {
        RawImage healthBarRawImage;
        EnemyAI enemy;

        // Use this for initialization
        void Start ()
        {
            enemy = GetComponentInParent<EnemyAI>(); // Different to way player's health bar finds player
            healthBarRawImage = GetComponent<RawImage>();
        }

        // Update is called once per frame
        void Update ()
        {
            float xValue = -(enemy.GetComponent<HealthSystem>().HealthAsPercentage / 2f) - 0.5f;
            healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }
    }
}