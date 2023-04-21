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

using System;

public class PlayerShootingXR : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem gunParticles;

    [SerializeField]
    private LineRenderer gunLine;

    [SerializeField]
    private AudioSource gunAudio;

    [SerializeField]
    private Light gunLight;

    [SerializeField]
    private Light faceLight;

    [SerializeField]
    private int damagePerShot = 20;

    [SerializeField]
    private float timeBetweenBullets = 0.15f;

    [SerializeField]
    private float range = 100f;

    private float timerFromLastBullet;

    private Ray shootRay = new Ray();
    private RaycastHit shootHit;

    [SerializeField]
    private string shootableMaskLayerName = "Shootable";
    private int shootableMask;

    private readonly float effectsDisplayTime = 0.2f;

    internal bool isOn = true;

    public Action<bool> OnShooting;

    private readonly string shootButtonName = "Fire1";

    private void Awake()
    {
        shootableMask = LayerMask.GetMask(shootableMaskLayerName);
    }

    private void Update()
    {
        timerFromLastBullet += Time.deltaTime;

        if (isOn
            && Input.GetButton(shootButtonName)
            && timerFromLastBullet >= timeBetweenBullets)
        {
            Shoot();

            OnShooting?.Invoke(true);
        }
        else
        {
            OnShooting?.Invoke(false);
        }

        if (timerFromLastBullet >= timeBetweenBullets * effectsDisplayTime)
        {
            SetEffectsActive(false);
        }
    }

    public void SetEffectsActive(bool isActive)
    {
        gunLine.enabled = isActive;
        faceLight.enabled = isActive;
        gunLight.enabled = isActive;
    }

    private void Shoot()
    {
        timerFromLastBullet = 0f;

        gunAudio.Play();

        gunLight.enabled = true;
        faceLight.enabled = true;

        // Stop the particles from playing if they were,
        // then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position
        // to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        // Set the shootRay so that it starts at the end
        // of the gun and points forward from the barrel.
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        // Perform the raycast against gameobjects
        // on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            // Try and find an EnemyHealth script on the gameobject hit.
            EnemyHealthXR enemyHealth =
                shootHit.collider.GetComponent<EnemyHealthXR>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            }

            // Set the second position of the line renderer
            // to the point the raycast hit.
            gunLine.SetPosition(1, shootHit.point);
        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            // ... set the second position of the line renderer
            // to the fullest extent of the gun's range.
            gunLine.SetPosition(
                1, shootRay.origin + shootRay.direction * range);
        }
    }
}
