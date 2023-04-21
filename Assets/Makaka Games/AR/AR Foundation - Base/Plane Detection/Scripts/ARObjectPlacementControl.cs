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
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using System.Collections.Generic;

[HelpURL("https://makaka.org/unity-assets")]
public class ARObjectPlacementControl : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager arRaycastManager;

    private static List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    private GameObject placedPrefab;

    /// <summary>
    /// The object instantiated as a result of
    /// a successful raycast intersection with a plane.
    /// </summary>
    private GameObject placedObject;

    [SerializeField]
    private bool isPlacedObjectRotatedToCamera = true;

    [Space]
    [SerializeField]
    private UnityEvent<Vector3> OnHitPoseChanged = null;

    [Space]
    [SerializeField]
    private UnityEvent OnObjectPlaced = null;

    private bool isActivated = false;

    private Transform lookAtTarget;

    public void AddObjectOnDetectedPlane(InputAction.CallbackContext context)
    {
        if (isActivated)
        {
            Vector2 touchPosition = context.ReadValue<Vector2>();

            if (IsPointerOverUIObject(touchPosition))
            {
                DebugPrinter.Print("UI object blocks AR Raycast Hit");
            }
            else
            {
                if (arRaycastManager.Raycast(touchPosition, hitResults,
                    TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    Pose hitPose = hitResults[0].pose;

                    OnHitPoseChanged.Invoke(hitPose.position);

                    if (placedObject)
                    {
                        if (!placedObject.activeSelf)
                        {
                            placedObject.SetActive(true);

                            DebugPrinter.Print("OnObjectPlaced.Invoke()");

                            OnObjectPlaced.Invoke();
                        }

                        placedObject.transform.position = hitPose.position;
                    }
                    else
                    {
                        placedObject = Instantiate(
                            placedPrefab, hitPose.position, hitPose.rotation);

                        if (placedObject)
                        {
                            DebugPrinter.Print("OnObjectPlaced.Invoke()");

                            OnObjectPlaced.Invoke();
                        }
                    }

                    if (isPlacedObjectRotatedToCamera)
                    {
                        placedObject.transform.LookAt(lookAtTarget);
                        placedObject.transform.eulerAngles = new Vector3(
                            0f, placedObject.transform.eulerAngles.y, 0f);
                    }
                }
            }
        }
    }

    private bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        PointerEventData pointerEventData =
            new PointerEventData(EventSystem.current)
            {
                position = touchPosition
            };

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        return raycastResults.Count > 0;
    }

    internal void Init(Transform lookAtTarget)
    {
        SetActivated(true);

        this.lookAtTarget = lookAtTarget;
    }

    internal void Stop()
    {
        SetActivated(false);

        DeactivatePlacedObject();
    }

    internal void DeactivatePlacedObject()
    {
        if (placedObject)
        {
            placedObject.SetActive(false);
        }
    }

    private void SetActivated(bool isActivated)
    {
        this.isActivated = isActivated;

        arRaycastManager.enabled = isActivated;
    }
}
