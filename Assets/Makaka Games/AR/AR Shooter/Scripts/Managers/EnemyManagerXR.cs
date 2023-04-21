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
using UnityEngine.Events;
using System.Collections;

public class EnemyManagerXR : MonoBehaviour
{
    [SerializeField]
    private RandomObjectPooler randomObjectPooler;

    [Space]
    public UnityEvent OnInitialized;

    [Header("Spawn")]
    [SerializeField]
    private float secondsToNextEnemy = 4f;

    [SerializeField]
    private float delayAtFirstStart = 2f;

    [SerializeField]
    private Transform[] spawnPoints;
    private int spawnPointIndex;

    private GameObject gameObjectTemp;
    private EnemyHealthXR enemyHealthXRTemp;

    private Coroutine spawnCoroutineReference;

    private float timeOnCoroutineStart = 0f;
    private float timeOnEnemyReset = 0f;
    private float pivotTime;

    private float secondsBetweenPivotTimeAndTimeOnPause = 0f;
    private float secondsToNextEnemyRemainder = 0f;

    private void Start()
    {
        randomObjectPooler.OnInitialized.AddListener(Init);
        randomObjectPooler.enabled = true;
    }

    /// <summary>Call after pool initialisation.</summary>
    public void Init()
    {
        StartCoroutine(InitCoroutine());
    }

    private IEnumerator InitCoroutine()
    {
        if (randomObjectPooler)
        {
            randomObjectPooler.InitControlScripts(typeof(EnemyHealthXR));

            for (int i = 0; i < randomObjectPooler.pooledObjects.Count; i++)
            {
                gameObjectTemp = randomObjectPooler.pooledObjects[i];

                if (gameObjectTemp)
                {
                    gameObjectTemp.SetActive(true);

                    enemyHealthXRTemp =
                        randomObjectPooler.RegisterControlScript(gameObjectTemp)
                            as EnemyHealthXR;

                    enemyHealthXRTemp.SetRenderersEnabled(false);

                    yield return new WaitForFixedUpdate();

                    gameObjectTemp.SetActive(false);

                    yield return new WaitForFixedUpdate();
                }
            }

            OnInitialized.Invoke();
        }
        else
        {
            Debug.LogError(
                "Random Object Pooler is Null. Assign it in the Editor.");
        }
    }

    public void Spawn(float? deltaTimeFromPause)
    {
        //Debug.Log("Spawn!");

        if (spawnCoroutineReference != null)
        {
            StopCoroutine(spawnCoroutineReference);

            spawnCoroutineReference = null;
        }

        spawnCoroutineReference =
            StartCoroutine(SpawnCoroutine(deltaTimeFromPause));
    }

    private IEnumerator SpawnCoroutine(float? timeOnPause)
    {
        // calculating the remaining time until the next spawning of the enemy,
        // taking into account the pause

        if (timeOnPause.HasValue)
        {
            pivotTime = timeOnEnemyReset > timeOnCoroutineStart
                ? timeOnEnemyReset
                : timeOnCoroutineStart;

            secondsBetweenPivotTimeAndTimeOnPause =
                timeOnPause.Value - pivotTime;

            secondsToNextEnemyRemainder -=
                secondsBetweenPivotTimeAndTimeOnPause;

            if (secondsToNextEnemyRemainder < 0f)
            {
                secondsToNextEnemyRemainder = 0f;
            }
        }
        else
        {
            secondsToNextEnemyRemainder = delayAtFirstStart;
        }

        timeOnCoroutineStart = Time.time;

        //DebugPrinter.Print("SpawnCoroutine: secondsToNextEnemy: "
        //    + secondsToNextEnemyRemainder);

        // delay at start or delay before next enemy

        yield return new WaitForSeconds(secondsToNextEnemyRemainder);

        while (PlayerHealthXR.Current.currentHealth > 0f
            && !PlayerHealthXR.Current.isOutOfSafeZone)
        {
            gameObjectTemp = randomObjectPooler.GetPooledObject();

            if (gameObjectTemp)
            {
                gameObjectTemp.SetActive(true);

                enemyHealthXRTemp =
                    randomObjectPooler.RegisterControlScript(gameObjectTemp)
                        as EnemyHealthXR;

                spawnPointIndex = Random.Range(0, spawnPoints.Length);

                enemyHealthXRTemp.ResetEnemy(
                    spawnPoints[spawnPointIndex].position,
                    spawnPoints[spawnPointIndex].rotation
                    );

                timeOnEnemyReset = Time.time;

                secondsToNextEnemyRemainder = secondsToNextEnemy;
            }

            yield return new WaitForSeconds(secondsToNextEnemy);
        }
    }
}