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
using UnityEngine.XR.ARFoundation;

using System.Collections;

[HelpURL("https://makaka.org/unity-assets")]
public class ARPlaneDetectionControl : MonoBehaviour
{
    [SerializeField]
    private ARSession arSession;

    [SerializeField]
    private ARInputManager arInputManager;

    [SerializeField]
    private ARSessionOrigin arSessionOrigin;

    [SerializeField]
    private ARPlaneManager arPlaneManager;

    [SerializeField]
    private ARPointCloudManager arPointCloudManager;

    [SerializeField]
    private ARCameraManager arCameraManager;

    private bool isFirstCameraFrameReceivedNotInEditor = false;

    [Space]
    [SerializeField]
    private ARObjectPlacementControl arObjectPlacementControl;

    [Space]
    [Header("UI")]
    [SerializeField]
    private GameObject canvasTutorial;

    [SerializeField]
    private GameObject canvasConfirmation;

    private Vector3 posOfSpawnedObjectOnDetectedPlane;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    private UnityEvent OnInitializedNotInEditor;

    [SerializeField]
    private UnityEvent OnInitializedInEditor;

    [SerializeField]
    private UnityEvent OnCameraFrameFirstReceivedNotInEditor;

    [Header("Detection Start")]
    [Space]
    [SerializeField]
    private UnityEvent OnDetectionStartedNotInEditor;

    [SerializeField]
    private UnityEvent OnDetectionStartedInEditor;

    [Header("Confirmation with Detected Point")]
    [Space]
    [SerializeField]
    private UnityEvent<Vector3> OnPlaneConfirmedWithDetectedPointNotInEditor;

    [SerializeField]
    private UnityEvent<Vector3> OnPlaneConfirmedWithDetectedPointInEditor;

    [Header("Confirmation with Camera")]
    [Space]
    [SerializeField]
    private UnityEvent<Transform> OnPlaneConfirmedWithCameraNotInEditor;

    [SerializeField]
    private UnityEvent<Transform> OnPlaneConfirmedWithCameraInEditor;

    [Header("Reset")]
    [Space]
    [SerializeField]
    private UnityEvent OnResetARSessionNotInEditor;

    [SerializeField]
    private UnityEvent OnResetARSessionInEditor;

    private IEnumerator Start()
    {
        arInputManager.enabled = true;

        canvasTutorial.SetActive(true);

        yield return null;

        arSessionOrigin.gameObject.SetActive(true);

        yield return null;

        arCameraManager.frameReceived += OnCameraFrameReceived;

#if UNITY_EDITOR

        StartInEditor();

#else

        StartNotInEditor();

#endif

    }

    private void OnDestroy()
    {
        //DebugPrinter.Print("\n ARPlaneDetectionControl - OnDestroy()");

        arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void StartInEditor()
    {
        OnInitializedInEditor.Invoke();
    }

    private void StartNotInEditor()
    {
        OnInitializedNotInEditor.Invoke();
    }

    public void StartPlaneDetection()
    {
        DebugPrinter.Print("\n StartPlaneDetection()");

        arSession.Reset();

        canvasTutorial.SetActive(false);

        arPlaneManager.enabled = true;

        arPointCloudManager.enabled = true;

        arObjectPlacementControl.Init(arCameraManager.transform);

#if UNITY_EDITOR

        OnDetectionStartedInEditor.Invoke();

#else

        OnDetectionStartedNotInEditor.Invoke();

#endif

    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!isFirstCameraFrameReceivedNotInEditor)
        {
            DebugPrinter.Print(
                "\n AR Foundation Camera - First Frame Received");

            isFirstCameraFrameReceivedNotInEditor = true;

            OnCameraFrameFirstReceivedNotInEditor.Invoke();
        }
    }

    public void SavePositionOfSpawnedObjectOnDetectedPlane(
        Transform transformForPos)
    {
        SavePositionOfSpawnedObjectOnDetectedPlane(transformForPos.position);
    }

    public void SavePositionOfSpawnedObjectOnDetectedPlane(Vector3 pos)
    {
        posOfSpawnedObjectOnDetectedPlane = pos;

        canvasConfirmation.SetActive(true);
    }

    public void ConfirmDetectedPlane()
    {
        canvasConfirmation.SetActive(false);

        arObjectPlacementControl.Stop();

        // detected planes will be stay on them places
        arPlaneManager.SetTrackablesActive(false);
        arPlaneManager.enabled = false;

        arPointCloudManager.SetTrackablesActive(false);
        arPointCloudManager.enabled = false;

#if UNITY_EDITOR

        OnPlaneConfirmedInEditor();

#else

        OnPlaneConfirmedNotInEditor();

#endif

    }

    private void OnPlaneConfirmedNotInEditor()
    {
        OnPlaneConfirmedWithDetectedPointNotInEditor.Invoke(
            posOfSpawnedObjectOnDetectedPlane);

        OnPlaneConfirmedWithCameraNotInEditor.Invoke(arCameraManager.transform);

    }

    private void OnPlaneConfirmedInEditor()
    {
        OnPlaneConfirmedWithDetectedPointInEditor.Invoke(
            posOfSpawnedObjectOnDetectedPlane);

        OnPlaneConfirmedWithCameraInEditor.Invoke(arCameraManager.transform);
    }

    public void ResetARSession()
    {
        arObjectPlacementControl.DeactivatePlacedObject();

        arSession.Reset();

#if UNITY_EDITOR

        ResetARSessionInEditor();

#else

        ResetARSessionNotInEditor();

#endif

    }

    private void ResetARSessionInEditor()
    {
        OnResetARSessionInEditor.Invoke();
    }

    private void ResetARSessionNotInEditor()
    {
        canvasConfirmation.SetActive(false);

        OnResetARSessionNotInEditor.Invoke();
    }

    public void PlaceObjectOnConfirmedPlaneUnderCamera(Transform targetObject)
    {
        arSessionOrigin.MakeContentAppearAt(
            targetObject,
            new Vector3(
                arCameraManager.transform.position.x,
                posOfSpawnedObjectOnDetectedPlane.y,
                arCameraManager.transform.position.z));
    }

    public void PlaceObjectOnConfirmedPlaneUnderDetectedPoint(
        Transform targetObject)
    {
        arSessionOrigin.MakeContentAppearAt(
            targetObject,
            posOfSpawnedObjectOnDetectedPlane);
    }

    public void MoveObjectXZLocalPosToCamera(
        Transform targetObject)
    {
        targetObject.transform.localPosition = new Vector3(
            arCameraManager.transform.position.x
                - arSessionOrigin.transform.position.x,
            targetObject.transform.localPosition.y,
            arCameraManager.transform.position.z
                - arSessionOrigin.transform.position.z);
    }

    public void SetARScaleInverted(float scale = 2f)
    {
        arSessionOrigin.transform.localScale =
            Vector3.one * scale;
    }

}
