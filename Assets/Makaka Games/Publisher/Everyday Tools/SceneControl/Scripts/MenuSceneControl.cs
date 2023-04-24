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

using System.Diagnostics;
using System.Collections.Generic;	

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[HelpURL("https://makaka.org/unity-assets")]
public class MenuSceneControl : MonoBehaviour 
{
	[SerializeField]
	private string nameOfSceneWithLoadScreen = "LoadScreen";

    public void LoadSceneWithScreenOrientationLandscapeLeft(string sceneName)
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;

		LoadScene(sceneName);
	}

	public void LoadSceneWithScreenOrientationPortrait(string sceneName)
	{
		Screen.orientation = ScreenOrientation.Portrait;

		LoadScene(sceneName);
	}

	public void LoadSceneWithScreenOrientationAuto(string sceneName)
	{
		Screen.orientation = ScreenOrientation.AutoRotation;

		LoadScene(sceneName);
	}

	public void LoadScene(string sceneName)
    {
		LoadScreenControl.Instance.LoadScene(
			sceneName, false, nameOfSceneWithLoadScreen);
	}

	public void ReloadCurrentScene()
	{
		LoadScreenControl.Instance.LoadScene(
			SceneManager.GetActiveScene().name,
			false,
			nameOfSceneWithLoadScreen);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void OpenLink(string link = "https://makaka.org/support")
	{
		Application.OpenURL(link);
	}
}