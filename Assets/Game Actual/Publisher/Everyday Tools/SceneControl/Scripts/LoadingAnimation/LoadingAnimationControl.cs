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
using UnityEngine.UI;

[HelpURL("https://makaka.org/unity-assets")]
public class LoadingAnimationControl : MonoBehaviour
{
    [SerializeField]
    private RectTransform fillAreaTransform;

    [SerializeField]
    private Image fillArea;

    [Header("Speed")]
    [SerializeField]
    private float rotationSpeed = 200f;

    [SerializeField]
    private float openSpeed = 0.005f;

    [SerializeField]
    private float closeSpeed = 0.01f;

    [Header("Size")]
    [SerializeField]
    private float sizeOnTop = 0.30f;

    [SerializeField]
    private float sizeOnBottom = 0.02f;

    private float fillAreaCurrentSize;

    private bool isFillAreaOnTop = true;

    private void Update()
    {
        fillAreaTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        ChangeFillAreaSize();
    }

    private void ChangeFillAreaSize()
    {
        fillAreaCurrentSize = fillArea.fillAmount;

        if (fillAreaCurrentSize < sizeOnTop && isFillAreaOnTop)
        {
            fillArea.fillAmount += openSpeed;
        }
        else if (fillAreaCurrentSize >= sizeOnTop && isFillAreaOnTop)
        {
            isFillAreaOnTop = false;
        }
        else if (fillAreaCurrentSize >= sizeOnBottom && !isFillAreaOnTop)
        {
            fillArea.fillAmount -= closeSpeed;
        }
        else if (fillAreaCurrentSize < sizeOnBottom && !isFillAreaOnTop)
        {
            isFillAreaOnTop = true;
        }
    }

}