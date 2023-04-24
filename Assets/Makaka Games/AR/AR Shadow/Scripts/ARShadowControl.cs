using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class ARShadowControl : MonoBehaviour
{
	[Header("UI")]

	[SerializeField]
	private GameObject canvasPause;

	[SerializeField]
	private GameObject canvasStart;

	[Space]
	[SerializeField]
	private GameObject canvasesHUD;

	[Header("Game")]

	[Space]
	[SerializeField]
	private UnityEvent OnARFoundationGameWorldInitialization;

	[SerializeField]
	private UnityEvent OnARFoundationGameWorldScaling;

	[SerializeField]
	private UnityEvent OnInitialized;

	[SerializeField]
	private UnityEvent OnGameRestarted;

	private bool isFirstStart = true;

	public void SetARFoundationReady()
	{
		canvasesHUD.SetActive(false);
	}

	public void InitGameForARFoundationWithDetectedPoint(Vector3 point)
	{
		StartCoroutine(
			InitGameForARFoundationWithDetectedPointCoroutine(point));
	}

	private IEnumerator InitGameForARFoundationWithDetectedPointCoroutine(
		Vector3 point)
	{
		canvasStart.SetActive(true);

		canvasesHUD.SetActive(true);

		yield return null;

		OnARFoundationGameWorldInitialization.Invoke();

		yield return null;

		OnARFoundationGameWorldScaling.Invoke();

		yield return null;

		OnInitialized.Invoke();
	}

	public void RestartGame()
	{
		canvasStart.SetActive(false);

		if (isFirstStart)
		{
			isFirstStart = false;
		}

		OnGameRestarted.Invoke();
	}

	public void PauseGameWhenPlayerLeftSafeZone()
	{
		canvasPause.SetActive(true);

		if (isFirstStart)
		{
			canvasStart.SetActive(false);
		}
	}

	public void PlayGameWhenPlayerEnteredSafeZone()
	{
		canvasPause.SetActive(false);

		if (isFirstStart)
		{
			canvasStart.SetActive(true);
		}
	}
}
