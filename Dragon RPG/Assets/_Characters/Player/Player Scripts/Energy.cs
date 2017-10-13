using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        // Object References
        CameraRaycaster cameraRaycaster;
        PlayerMovement playerMovement;

        // Constants
        const int ENEMY_LAYER = 9;

        // Variables
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100;
        [SerializeField] float pointsPerHit = 10;
        float currentEnergyPoints;

        // Use this for initialization
        void Start ()
        {
            RegisterObservers();
            playerMovement = FindObjectOfType<PlayerMovement>();
            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyBar();
        }

        void RegisterObservers ()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.notifyRightClickObservers += UseEnergy;
        }

        void UseEnergy (RaycastHit raycastHit, int layerHit)
        {
            if (layerHit == ENEMY_LAYER)
            {
                currentEnergyPoints = Mathf.Clamp(currentEnergyPoints -= pointsPerHit, 0, maxEnergyPoints);
                playerMovement.SetTarget(raycastHit);
                UpdateEnergyBar();
            }
        }

        void UpdateEnergyBar ()
        {
            float energyPointsAsPercentage = currentEnergyPoints / maxEnergyPoints;
            float xValue = -(energyPointsAsPercentage / 2f) - .5f;
            energyBar.uvRect = new Rect(xValue, 0f, .5f, 1f);
        }
    }
}