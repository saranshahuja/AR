/*
===================================================================
Unity Assets by MAKAKA GAMES: https://makaka.org/o/all-unity-assets
===================================================================

Online Docs (Latest): https://makaka.org/unity-assets
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHealthXR : MonoBehaviour
{
    [SerializeField]
    private PlayerShootingXR playerShooting;

    [SerializeField]
    private Animator anim;

    private const string animationTriggerNameDie = "Die";
    private const string animationTriggerNameReset = "Reset";
    private const string animationBoolNameIsWalking = "IsWalking";

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip deathClip;

    [SerializeField]
    private AudioClip hurtClip;

    [Header("Health")]
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private int startingHealth = 100;
    public int currentHealth;

    [Header("Damage")]
    [SerializeField]
    private Image damageImage;

    [SerializeField]
    private Color damageImageColorFlash = new Color(1f, 0f, 0f, 0.223f);
    private Color damageImageColorDefault;

    [SerializeField]
    private float damageImageColorFlashSpeed = 40f;

    public GameObject damageZone;

    private bool isDamaged = false;

    [Header("Safe Zone")]
    [SerializeField]
    private GameObject safeZone;

    internal bool isOutOfSafeZone = false;
    private bool isFirstEnterToSafeZone = true;

    [Space]
    [SerializeField]
    private UnityEvent OnSafeZoneEnter;

    [SerializeField]
    private UnityEvent OnSafeZoneExit;

    [Header("Death")]
    [Space]
    [SerializeField]
    private UnityEvent OnDeath;

    private bool isDead = false;

    public static PlayerHealthXR Current;

    private void Awake()
    {
        currentHealth = startingHealth;

        damageImageColorDefault = damageImage.color;

        playerShooting.OnShooting += AnimateWalking;

        Current = this;
    }

    private void Update()
    {
        if (isDamaged)
        {
            damageImage.color = Color.Lerp(
                damageImage.color, damageImageColorFlash, damageImageColorFlashSpeed * Time.deltaTime);
        }
        else
        {
            damageImage.color = damageImageColorDefault;
        }

        isDamaged = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == safeZone)
        {
            if (isFirstEnterToSafeZone)
            {
                isFirstEnterToSafeZone = false;
            }
            else
            {
                isOutOfSafeZone = false;

                playerShooting.isOn = true;

                OnSafeZoneEnter.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == safeZone)
        {
            isOutOfSafeZone = true;

            playerShooting.isOn = false;

            OnSafeZoneExit.Invoke();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isOutOfSafeZone)
        {
            isDamaged = true;

            currentHealth -= amount;

            healthSlider.value = currentHealth;

            audioSource.Play();

            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isDead = true;

        playerShooting.SetEffectsActive(false);

        anim.SetTrigger(animationTriggerNameDie);

        audioSource.clip = deathClip;
        audioSource.Play();

        playerShooting.enabled = false;

        OnDeath?.Invoke();
    }

    public void ResetPlayer()
    {
        currentHealth = startingHealth;

        healthSlider.value = currentHealth;

        isDead = false;

        audioSource.clip = hurtClip;

        anim.SetTrigger(animationTriggerNameReset);

        playerShooting.enabled = true;
        playerShooting.SetEffectsActive(true);
    }

    public void AnimateWalking(bool isAnimated)
    {
        anim.SetBool(animationBoolNameIsWalking, isAnimated);
    }
}
