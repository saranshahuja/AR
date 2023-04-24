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

public class EnemyAttackXR : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    private readonly string animParameterNameForPlayerDead = "PlayerDead";

    [SerializeField]
    private EnemyHealthXR enemyHealth;

    private bool playerInRange = false;
    private bool PlayerInRange
    {
        get => playerInRange;
        set
        {
            playerInRange = value;

            //DebugPrinter.Print(
            //    $"playerInRange = {PlayerInRange} on {gameObject.name};");
        }
    }

    [SerializeField]
    private float timeBetweenAttacks = 0.5f;

    [SerializeField]
    private int attackDamage = 10;

    private float timerFromLastAttack;

    private void OnEnable()
    {
        // Because OnTriggerExit() doesn't work when Deactivating
        PlayerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerHealthXR.Current.damageZone)
        {
            PlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerHealthXR.Current.damageZone)
        {
            PlayerInRange = false;
        }
    }

    private void Update()
    {
        timerFromLastAttack += Time.deltaTime;

        if (timerFromLastAttack >= timeBetweenAttacks
            && PlayerInRange
            && enemyHealth.currentHealth > 0)
        {
            Attack();
        }

        if (PlayerHealthXR.Current.currentHealth <= 0)
        {
            anim.SetTrigger(animParameterNameForPlayerDead);
        }
    }

    private void Attack()
    {
        //DebugPrinter.Print($"Attack by {gameObject.name}");

        timerFromLastAttack = 0f;

        if (PlayerHealthXR.Current.currentHealth > 0)
        {
            PlayerHealthXR.Current.TakeDamage(attackDamage);
        }
    }
}