using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        // Components
        [SerializeField] Image healthBar;
        Animator animator;
        AudioSource audioSource;
        Character character;

        // Variables
        [SerializeField] float maxHealthPoints = 1000f;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float deathVanishSeconds = 2f;
        const string DEATH_TRIGGER = "Death";
        float currentHealthPoints;
        public float HealthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        public float HealthDifference
        {
            get
            {
                return maxHealthPoints - currentHealthPoints;
            }
        }

        // Use this for initialization
        void Start ()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            character = GetComponent<Character>();

            currentHealthPoints = maxHealthPoints;
        }

        // Update is called once per frame
        void Update ()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar ()
        {
            if (healthBar)
            {
                healthBar.fillAmount = HealthAsPercentage;
            }
        }

        public void TakeDamage (float damage)
        {
            bool characterDies = currentHealthPoints - damage <= 0;
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
            else
            {
                if (damageSounds.Length > 0)
                {
                    AudioClip clip = damageSounds[Random.Range(0, damageSounds.Length)];
                    audioSource.PlayOneShot(clip);
                }
            }
        }

        IEnumerator KillCharacter ()
        {
            character.Kill();
            animator.SetTrigger(DEATH_TRIGGER);

            PlayerMovement playerComponent = GetComponent<PlayerMovement>();
            if (playerComponent && playerComponent.isActiveAndEnabled)
            {
                audioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
                audioSource.Play();
                yield return new WaitForSecondsRealtime(audioSource.clip.length);
                SceneManager.LoadScene(0);
            }
            else
            {
                Destroy(gameObject, deathVanishSeconds);
                StopAllCoroutines();
            }
        }

        public void Heal (float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0, maxHealthPoints);
        }
    }
}