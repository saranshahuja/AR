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

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

#if UNITY_IOS

using UnityEngine.iOS;

#endif

#pragma warning disable 649

/// <summary>
/// There is a checking for AR support before the ARSession is enabled.
/// For ARCore in particular, it is possible for a device to support ARCore
/// but not have it installed. This example will detect this case and prompt
/// the user to install ARCore. To test this feature yourself, use a supported
/// device and uninstall ARCore.
/// (Settings > Search for "ARCore" and uninstall or disable it.)
/// </summary>

[HelpURL("https://makaka.org/unity-assets")]
public class ARFoundationSupportChecker : MonoBehaviour
{
    [SerializeField]
    private ARSession arSession = null;

    [SerializeField]
    private GameObject canvasOnNoAR = null;

    private Action OnClickOfActionButton;

    [Header("Script Works for iOS & Android")]

    [Tooltip("Is AR Foundation Support checked in Editor" +
        " to Test initial UI without Building?")]
    [SerializeField]
    private bool isCheckedInEditorOnInit;

    [Tooltip("iPhone X and higher.")]
    [SerializeField]
    private bool isCheckedFaceTrackingForIOS;

    [SerializeField]
    private bool isARUnsupportedNotInEditorTest;

    [Space]

    [SerializeField]
    private UnityEvent OnStartedNotInEditor;

    [Space]

    [Header("Cross-Platform Events")]

    [SerializeField]
    private UnityEvent OnARReady = null;

    // String - Status Text
    [SerializeField]
    private UnityEventString OnARUnsupported = null;

    [Space]

    [Header("Android Only Events")]

    [Space]

    // String - Status Text
    [SerializeField]
    private UnityEventString OnARSoftwareUpdateFailed = null;

    [Serializable]
    private class UnityEventString : UnityEvent<string> { }

    [Header("Requirements")]

    [SerializeField]
    private string RequirementsForNotIOSAndroid;

    // Face Tracking iOS: https://developer.apple.com/documentation/arkit/tracking_and_visualizing_faces#3526598
    // Image Tracking iOS: https://developer.apple.com/documentation/arkit/detecting_images_in_an_ar_experience

    [SerializeField]
    private string RequirementsForIOS;

    // Android: https://developers.google.com/ar/devices

    [SerializeField]
    private string RequirementsForAndroid;

    [Header("Requirements: URLs")]

    [SerializeField]
    private string RequirementsURLForIOS;

    [SerializeField]
    private string RequirementsURLForAndroid;

    [SerializeField]
    private string RequirementsURLForNotIOSAndroid;

    private void Start()
    {
        arSession.gameObject.SetActive(true);
        arSession.enabled = false;

#if UNITY_EDITOR

#if UNITY_IOS || UNITY_ANDROID

        if (isCheckedInEditorOnInit)
        {
            Check();
        }
        else
        {
            SuccessTest();
        }
#else

        InitARIsNotSupported();

#endif

#else

        OnStartedNotInEditor?.Invoke();

        if (isARUnsupportedNotInEditorTest)
        {
            InitARIsNotSupported();
        }
        else
        {
            Check();
        }

#endif

    }

    private void Check()
    {
        StartCoroutine(CheckCoroutine());
    }

    private IEnumerator CheckCoroutine()
    {
        SetCanvasOnNoARActive(false);

        DebugPrinter.Print("\n ▶▶▶ ARF: Checking for support...");

        yield return ARSession.CheckAvailability();

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            DebugPrinter.Print(
                "\n ▶▶▶ ARF: is Supported, but requires a software update.");

            DebugPrinter.Print("\n ▶▶▶ ARF: Attempting install...");

            yield return ARSession.Install();
        }

        if (ARSession.state == ARSessionState.Ready)
        {
            DebugPrinter.Print("\n ▶▶▶ ARF: is Supported!");
            DebugPrinter.Print("\n ▶▶▶ ARF: Starting AR session...");

#if UNITY_IOS

            if (isCheckedFaceTrackingForIOS)
            {
                if (IsFaceTrackingSupportedForIOS())
                {
                    DebugPrinter.Print(
                        "\n ▶▶▶ ARF: Face Tracking is Supported!");

                    Success();
                }
                else
                {
                    DebugPrinter.Print(
                        "\n ▶▶▶ ARF: Face Tracking is NOT Supported!");

                    InitARIsNotSupported();
                }
            }
            else
            {
                Success();
            }

#else

            Success();

#endif

        }
        else
        {
            switch (ARSession.state)
            {
                case ARSessionState.Unsupported:

                    InitARIsNotSupported();

                    break;

                case ARSessionState.NeedsInstall: // Android Only

                    FailInstall();

                    break;
            }
        }
    }

    private void InitARIsNotSupported()
    {
        OnClickOfActionButton = () =>
        {
            Application.OpenURL(GetRequirementsURL());
        };

        SetCanvasOnNoARActive(true);

        DebugPrinter.Print("\n ▶▶▶ ARF: " + GetRequirements());

        OnARUnsupported?.Invoke(GetRequirements());

        arSession.gameObject.SetActive(false);
    }

    public string GetRequirements()
    {

#if UNITY_IOS

        return RequirementsForIOS;

#elif UNITY_ANDROID

        return RequirementsForAndroid;

#else

        return RequirementsForNotIOSAndroid;

#endif

    }

    public string GetRequirementsURL()
    {

#if UNITY_IOS

        return RequirementsURLForIOS;

#elif UNITY_ANDROID

        return RequirementsURLForAndroid; 

#else

        return RequirementsURLForNotIOSAndroid;

#endif

    }

    private void Success()
    {
        arSession.enabled = true;

        OnARReady?.Invoke();
    }

    public void SuccessTest()
    {
        DebugPrinter.Print("\n ▶▶▶ ARF: is Supported: Success Test in Editor."
            + " Real Testing is only on Real Device.");

        SetCanvasOnNoARActive(false);

        Success();
    }

    /// <summary>
    /// Android Only.
    /// </summary>
    private void FailInstall()
    {
        DebugPrinter.Print("\n ▶▶▶ ARF: The software update failed," +
            " or you declined the update.");

        OnARSoftwareUpdateFailed?.Invoke(
            "The software update failed, or you declined the update.");

        // In this case, we enable a button which allows the user
        // to try again in the event they decline the update the first time.

        OnClickOfActionButton = () =>
        {
            InstallARSoftware();
        };

        SetCanvasOnNoARActive(true);
    }

    private void SetCanvasOnNoARActive(bool active)
    {
        if (canvasOnNoAR != null)
        {
            canvasOnNoAR.SetActive(active);
        }
    }

    public void ClickOnActionButton()
    {
        OnClickOfActionButton?.Invoke();
    }

    /// <summary>
    /// Android Only.
    /// </summary>
    public void InstallARSoftware()
    {
        StartCoroutine(InstallARSoftwareCoroutine());
    }

    /// <summary>
    /// Android Only.
    /// </summary>
    private IEnumerator InstallARSoftwareCoroutine()
    {
        SetCanvasOnNoARActive(false);

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            DebugPrinter.Print("\n ▶▶▶ ARF: Attempting install...");

            yield return ARSession.Install();

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                FailInstall();
            }
            else if (ARSession.state == ARSessionState.Ready)
            {
                Success();
            }
        }
        else
        {
            DebugPrinter.Print(
                "\n ▶▶▶ ARF: Error: AR Software does not require install.");
        }
    }

#if UNITY_IOS

    private bool IsFaceTrackingSupportedForIOS()
    {
        return !(
            Device.generation == DeviceGeneration.iPhone6S
        || Device.generation == DeviceGeneration.iPhone6SPlus
        || Device.generation == DeviceGeneration.iPhoneSE1Gen
        || Device.generation == DeviceGeneration.iPhone7
        || Device.generation == DeviceGeneration.iPhone7Plus
        || Device.generation == DeviceGeneration.iPhone8
        || Device.generation == DeviceGeneration.iPhone8Plus);
    }

#endif

}
