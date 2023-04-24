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

[HelpURL("https://makaka.org/unity-assets")]
public class ARPlayerControl : MonoBehaviour
{
    [SerializeField]
    private GameObject safeZone;

    internal bool isOutOfSafeZone = false;
    private bool isFirstEnterToSafeZone = true;

    [Space]
    [SerializeField]
    private UnityEvent OnSafeZoneEnter;

    [SerializeField]
    private UnityEvent OnSafeZoneExit;

    public static ARPlayerControl Current;

    private void Awake()
    {
        Current = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == safeZone)
        {
            if (isFirstEnterToSafeZone)
            {
                isFirstEnterToSafeZone = false;
            }
            else
            {
                isOutOfSafeZone = false;

                OnSafeZoneEnter.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == safeZone)
        {
            isOutOfSafeZone = true;

            OnSafeZoneExit.Invoke();
        }
    }
}
