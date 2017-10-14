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

        // Constants
        const int ENEMY_LAYER = 9;

        // Variables
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100;
        [SerializeField] float pointsPerHit = 10;
        float currentEnergyPoints;
        float xValueOffset = .5f;

        // Use this for initialization
        void Start ()
        {
            RegisterObservers();
            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyBar();
        }

        void RegisterObservers ()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.notifyMouseOverEnemy += UseEnergy;
        }

        void UseEnergy (Enemy enemy)
        {
            if (Input.GetMouseButtonDown(1) && Vector3.Distance(transform.position, enemy.transform.position) < 2f)
            {
                currentEnergyPoints = Mathf.Clamp(currentEnergyPoints -= pointsPerHit, 0, maxEnergyPoints);
                UpdateEnergyBar();
            }
        }

        void UpdateEnergyBar ()
        {
            float energyPointsAsPercentage = currentEnergyPoints / maxEnergyPoints;
            float xValue = -(energyPointsAsPercentage / 2f) - xValueOffset;
            energyBar.uvRect = new Rect(xValue, 0f, .5f, 1f);
        }
    }
}