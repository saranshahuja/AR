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
using UnityEngine.AI;
using System.Collections;

public class EnemyHealthXR : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    private readonly string animParameterNameForIsDead = "isDead";

    [Space]
    [SerializeField]
    private AudioSource enemyAudio;

    [SerializeField]
    private AudioClip hurtClip;

    [SerializeField]
    private AudioClip deathClip;

    [Space]
    [SerializeField]
    private ParticleSystem hitParticles;

    [SerializeField]
    private SphereCollider sphereColliderTrigger;

    [SerializeField]
    private Rigidbody rigidbody3D;

    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private Renderer[] renderers;

    [Space]
    [SerializeField]
    private int scoreValue = 10;

    [SerializeField]
    private int startingHealth = 100;
    public int currentHealth;

    [SerializeField]
    private float sinkDelay = 0.17f;

    [SerializeField]
    private float sinkSpeed = 2.5f;

    [SerializeField]
    private float deactivatingDelay = 0.3f;
    private bool isDead = false;
    private bool isSinking = false;

    private void Awake()
    {
        ResetHealth();
    }

    private void ResetHealth()
    {
        currentHealth = startingHealth;
    }

    private void Update()
    {
        if (isSinking)
        {
            transform.Translate(sinkSpeed * Time.deltaTime * -Vector3.up);
        }

        if (!isDead)
        {
            if (currentHealth > 0 && PlayerHealthXR.Current.currentHealth > 0)
            {
                if (PlayerHealthXR.Current.isOutOfSafeZone)
                {
                    navMeshAgent.ResetPath();
                }
                else
                {
                    navMeshAgent.SetDestination(
                        PlayerHealthXR.Current.transform.position);
                }
            }
            else
            {
                StartCoroutine(Death(true));
            }
        }
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (isDead)
        {
            return;
        }

        enemyAudio.Play();

        currentHealth -= amount;

        hitParticles.transform.position = hitPoint;

        hitParticles.Play();

        if (currentHealth <= 0)
        {
            StartCoroutine(Death(false));
        }
    }

    private IEnumerator Death(bool isGameOver)
    {
        //Debug.Log($"Death Coroutine: IsGameOver => {isGameOver}");

        isDead = true;

        sphereColliderTrigger.enabled = false;

        anim.SetBool(animParameterNameForIsDead, true);

        if (!isGameOver)
        {
            enemyAudio.clip = deathClip;
            enemyAudio.Play();

            ScoreManagerXR.Increase(scoreValue);
        }

        yield return new WaitForSeconds(sinkDelay);

        //Debug.Log("StartSinking");

        navMeshAgent.enabled = false;

        rigidbody3D.isKinematic = true;

        isSinking = true;

        yield return StartCoroutine(Deactivate());
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(deactivatingDelay);

        SetRenderersEnabled(false);

        yield return new WaitForSeconds(deactivatingDelay);

        gameObject.SetActive(false);

        //Debug.Log("Deactivate");
    }

    public void ResetEnemy(Vector3 position, Quaternion rotation)
    {
        StartCoroutine(ResetEnemyCoroutine(position, rotation));
    }

    private IEnumerator ResetEnemyCoroutine(Vector3 position, Quaternion rotation)
    {
        yield return new WaitForFixedUpdate();

        navMeshAgent.Warp(position);
        rigidbody3D.rotation = rotation;

        yield return new WaitForFixedUpdate();

        SetRenderersEnabled(true);

        ResetHealth();

        navMeshAgent.enabled = true;

        rigidbody3D.isKinematic = false;
        sphereColliderTrigger.enabled = true;

        isDead = false;
        isSinking = false;

        anim.SetBool(animParameterNameForIsDead, false);

        enemyAudio.clip = hurtClip;

    }

    public void SetRenderersEnabled(bool enabled)
    {
        if (renderers.Length > 0)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = enabled;
            }
        }
        else
        {
            Debug.Log("Assign Renderer for EnemyHealthXR!");
        }
    }
}