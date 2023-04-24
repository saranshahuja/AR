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
please first send email to info@makaka.org (in English or in Russian) 
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using System;
using System.Collections.Generic;

using UnityEngine;

[HelpURL("https://makaka.org/unity-assets")]
public class SameGameObjectDetector : MonoBehaviour
{
    [SerializeField]
    private string nameForLog;

    [SerializeField]
    private bool isDebugLogging = false;

    [Space]
    [Tooltip("During this time, the object is considered the same.")]
    [SerializeField]
    [Range(0f, 20f)]
    private float safeTime = 3f;
    private float currentTime;
    private float previousTime;

    private Dictionary<GameObject, float> objectsWithTimeStaps;
    private GameObject currentObject;

    private event Action OnInitialized;

    public void Init(int countOfObjectsForInteraction, Action OnInitialized)
    {
        this.OnInitialized += OnInitialized;

        // Memory Allocation before Game Start
        objectsWithTimeStaps =
            new Dictionary<GameObject, float>(countOfObjectsForInteraction);

        if (this.OnInitialized != null)
        {
            this.OnInitialized.Invoke();
        }
    }

    /// <summary>
    /// Register - Memorize the Time.
    /// </summary>
    public bool? DetectOrRegister(GameObject other)
    {
        if (objectsWithTimeStaps != null)
        {
            currentTime = Time.time;
            currentObject = other;

            if (!objectsWithTimeStaps.TryGetValue(
                currentObject, out previousTime))
            {
                if (isDebugLogging)
                {
                    DebugPrinter.Print("DetectOrRegister1 - Registration: "
                        + nameForLog);
                }

                objectsWithTimeStaps.Add(currentObject, currentTime);
            }

            //DebugPrinter.Print(
            //    $"{currentTime} - {previousTime} > {safeTime}");

            if (currentTime - previousTime > safeTime || previousTime < 0.001f)
            {
                if (isDebugLogging)
                {
                    DebugPrinter.Print("DetectOrRegister2 - from: "
                        + nameForLog + " - Diff Objects or Registration");
                }

                objectsWithTimeStaps[currentObject] = currentTime;

                return false;
            }
            else
            {
                if (isDebugLogging)
                {
                    DebugPrinter.Print("DetectOrRegister2 - from: "
                        + nameForLog + " - Same Objects");
                }

                return true;
            }
        }
        else
        {
            if (isDebugLogging)
            {
                DebugPrinter.Print("DetectOrRegisterX - from: "
                    + nameForLog + " - Not Initialized!");
            }

            return null;
        }
    }
}
