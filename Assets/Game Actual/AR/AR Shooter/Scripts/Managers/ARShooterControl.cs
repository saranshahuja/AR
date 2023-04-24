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

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class ARShooterControl : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSourceBGMusic;

    [Header("UI")]
    [SerializeField]
    private GameObject canvasPause;

    [SerializeField]
    private GameObject canvasStart;

    [SerializeField]
    private TextMeshProUGUI canvasStartTextInfo;

    [SerializeField]
    [TextArea(3, 4)]
    private string canvasStartTextInfoARFoundation;

    [SerializeField]
    [TextArea(3, 4)]
    private string canvasStartTextInfoARCameraGYRO;

    [SerializeField]
    [TextArea(3, 4)]
    private string canvasStartTextInfoARCameraGYROAccelerometer;

    [Space]
    [SerializeField]
    private GameObject canvasesHUD;

    [SerializeField]
    private Animator canvasGameOverAnimator;

    [SerializeField]
    private TextMeshProUGUI scoreTextValueGameOver;

    private readonly string animationTriggerNameGameOver = "GameOver";
    private readonly string animationTriggerNameRestart = "Restart";

    [Header("Game")]
    [SerializeField]
    private EnemyManagerXR[] enemyManagersXR;
    private EnemyManagerXR enemyManagerXRTemp;

    private int initedPools = 0;
    private int enemyManagersXRActiveCount = 0;

    private bool isFirstStart = true;
    private bool isGameRestarted = false;
    private float timeOnPause = 0f;

    [Space]
    [SerializeField]
    private UnityEvent OnARFoundationGameWorldInitialization;

    [SerializeField]
    private UnityEvent OnARFoundationGameWorldScaling;

    [SerializeField]
    private UnityEvent OnInitialized;

    [SerializeField]
    private UnityEvent OnGameRestarted;

    public void InitGameForARCameraGYRO(bool isAccelerometerMode = false)
    {
        StartCoroutine(InitGameForARCameraGYROCoroutine(isAccelerometerMode));
    }

    private IEnumerator InitGameForARCameraGYROCoroutine(
        bool isAccelerometerMode = false)
    {
        audioSourceBGMusic.Play();

        canvasStart.SetActive(true);

        canvasesHUD.SetActive(true);

        canvasStartTextInfo.text =
            isAccelerometerMode
            ? canvasStartTextInfoARCameraGYROAccelerometer
            : canvasStartTextInfoARCameraGYRO;

        yield return null;

        InitEnemies();
    }

    public void SetARFoundationReady()
    {
        canvasesHUD.SetActive(false);
    }

    public void InitGameForARFoundationWithCamera(Transform camera)
    {
        StartCoroutine(
            InitGameForARFoundationWithCameraCoroutine(camera));
    }

    private IEnumerator InitGameForARFoundationWithCameraCoroutine(
        Transform camera)
    {
        audioSourceBGMusic.Play();

        canvasStart.SetActive(true);
        canvasStartTextInfo.text = canvasStartTextInfoARFoundation;

        canvasesHUD.SetActive(true);

        Vector3 playerLocalPosLast =
            PlayerHealthXR.Current.transform.localPosition;

        Quaternion playerLocalRotLast =
            PlayerHealthXR.Current.transform.localRotation;

        PlayerHealthXR.Current.transform.parent = camera;
        PlayerHealthXR.Current.transform.localPosition = playerLocalPosLast;
        PlayerHealthXR.Current.transform.localRotation = playerLocalRotLast;

        yield return null;

        OnARFoundationGameWorldInitialization.Invoke();

        yield return null;

        OnARFoundationGameWorldScaling.Invoke();

        yield return null;

        InitEnemies();
    }

    public void RestartGame()
    {
        isGameRestarted = true;

        canvasStart.SetActive(false);

        if (isFirstStart)
        {
            isFirstStart = false;
        }
        else
        {
            canvasGameOverAnimator.SetTrigger(animationTriggerNameRestart);

            ScoreManagerXR.ResetScore();
        }

        PlayerHealthXR.Current.ResetPlayer();

        Spawn(null);

        OnGameRestarted.Invoke();
    }

    public void GameOver()
    {
        isGameRestarted = false;

        scoreTextValueGameOver.text = ScoreManagerXR.GetScore().ToString();

        canvasGameOverAnimator.SetTrigger(animationTriggerNameGameOver);
    }

    public void InitEnemies()
    {
        //DebugPrinter.Print("InitEnemies()");

        for (int i = 0; i < enemyManagersXR.Length; i++)
        {
            enemyManagerXRTemp = enemyManagersXR[i];

            if (enemyManagerXRTemp && enemyManagerXRTemp.gameObject.activeSelf)
            {
                enemyManagerXRTemp.OnInitialized.AddListener(
                    CountInitedEnemyManagerXR);

                enemyManagerXRTemp.enabled = true;

                enemyManagersXRActiveCount++;
            }
        }
    }

    public void CountInitedEnemyManagerXR()
    {
        initedPools++;

        //Debug.Log("Pool Inited: #" + initedPools);

        if (initedPools == enemyManagersXRActiveCount)
        {
            OnInitialized.Invoke();

            //Debug.Log("All Pools Inited — Game Initialized");
        }
    }

    public void Spawn(float? deltaTimeFromPause)
    {
        for (int i = 0; i < enemyManagersXR.Length; i++)
        {
            enemyManagerXRTemp = enemyManagersXR[i];

            if (enemyManagerXRTemp && enemyManagerXRTemp.gameObject.activeSelf)
            {
                enemyManagerXRTemp.Spawn(deltaTimeFromPause);
            }
        }
    }

    public void PauseGameWhenPlayerLeftSafeZone()
    {
        canvasPause.SetActive(true);

        if (isGameRestarted)
        {
            //DebugPrinter.Print("!!!Start Pause");

            timeOnPause = Time.time;
        }
        else
        {
            if (isFirstStart)
            {
                canvasStart.SetActive(false);
            }
            else
            {
                canvasGameOverAnimator.SetTrigger(animationTriggerNameRestart);
            }
        }
    }

    public void PlayGameWhenPlayerEnteredSafeZone()
    {
        canvasPause.SetActive(false);

        if (isGameRestarted)
        {
            //DebugPrinter.Print("!!!Stop Pause");

            Spawn(timeOnPause);
        }
        else
        {
            if (isFirstStart)
            {
                canvasStart.SetActive(true);
            }
            else
            {
                canvasGameOverAnimator.SetTrigger(animationTriggerNameGameOver);
            }
        }
    }
}
