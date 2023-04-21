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

[HelpURL("https://makaka.org/unity-assets")]
public class RotationByKeysControl : MonoBehaviour
{
    [Header("Horizontal")]
    [Tooltip("Object for Horizontal Rotation")]
    public Transform horizontal;
    public string horizontalAxis = "Horizontal";
    public float speedHorizontal = 75f;
    
    [Header("Vertical")]
    [Tooltip("Object for Vertical Rotation")]
    public Transform vertical;
    public string verticalAxis = "Vertical";
    public float speedVertical = -50f;

    private void LateUpdate()
    {
        horizontal.Rotate(
            0f, 
            Input.GetAxis(horizontalAxis) * speedHorizontal * Time.deltaTime, 
            0f);

        vertical.Rotate(
            Input.GetAxis(verticalAxis) * speedVertical * Time.deltaTime,
            0f, 
            0f);
    }

}
