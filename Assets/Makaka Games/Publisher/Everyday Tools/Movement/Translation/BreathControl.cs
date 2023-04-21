﻿/*
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
public class BreathControl : MonoBehaviour
{
    public Vector3 period = new Vector3(0f, 1.2f, 0f);

    public Vector3 amplitude = new Vector3(0f, 0.2f, 0f);
    
    private Vector3 distanceCurrent;
    
    private Vector3 positionOnStart;

    protected void Start()
    {
        positionOnStart = transform.position;
    }

    protected void Update()
    {
        distanceCurrent = new Vector3 (
            period.x == 0f ? 0f : Mathf.Sin (Time.timeSinceLevelLoad / period.x),
            period.y == 0f ? 0f : Mathf.Sin (Time.timeSinceLevelLoad / period.y),
            period.z == 0f ? 0f : Mathf.Sin (Time.timeSinceLevelLoad / period.z)
            );

        transform.position = positionOnStart + Vector3.Scale(amplitude, distanceCurrent);
    }

}
