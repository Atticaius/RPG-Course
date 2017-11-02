using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] Image energyOrb;
        [SerializeField] AudioClip outOfEnergySound;
        AudioSource audioSource;

        // Variables
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] float maxEnergyPoints = 100;
        [SerializeField] float regenPointsPerSecond = 1f;
        float currentEnergyPoints;
        public float abilityLength
        {
            get
            {
                return abilities.Length;
            }
        }
        public AbilityConfig[] GetAbilities
        {
            get
            {
                return abilities;
            }
        }

        // Use this for initialization
        void Start ()
        {
            audioSource = GetComponent<AudioSource>();
            currentEnergyPoints = maxEnergyPoints;
            AttachAbilities();
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
            energyOrb.fillAmount = energyPointsAsPercentage;
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

        void AttachAbilities ()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility (int abilityIndex, GameObject target = null)
        {
            var energyCost = abilities[abilityIndex].GetEnergyCost;

            if (energyCost <= currentEnergyPoints)
            {
                // transform.LookAt(target.transform);
                UseEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            } else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(outOfEnergySound);
                }
            }
        }
    }
}