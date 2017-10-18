using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        // Variables
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100;
        [SerializeField] float regenPointsPerSecond = 10f;
        float currentEnergyPoints;
        float xValueOffset = .5f;

        // Use this for initialization
        void Start ()
        {
            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyBar();
        }

        private void Update ()
        {
            RegenEnergy();
        }

        public bool IsEnergyAvailable (float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void UseEnergy (float amount)
        {
                currentEnergyPoints = Mathf.Clamp(currentEnergyPoints -= amount, 0, maxEnergyPoints);
                UpdateEnergyBar();
        }

        void UpdateEnergyBar ()
        {
            float energyPointsAsPercentage = currentEnergyPoints / maxEnergyPoints;
            float xValue = -(energyPointsAsPercentage / 2f) - xValueOffset;
            energyBar.uvRect = new Rect(xValue, 0f, .5f, 1f);
        }

        void RegenEnergy ()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                float pointsToAdd = regenPointsPerSecond * Time.deltaTime;
                currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
                UpdateEnergyBar();
            }
        }
    }
}