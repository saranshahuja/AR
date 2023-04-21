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

#pragma warning disable 649

[HelpURL("https://makaka.org/unity-assets")]
public class MaterialCounter : MonoBehaviour
{
    private static readonly string messageByDefault = "Materials Count:";
    private static readonly string messageByDefaultOnDestroy =
        "Materials Count — Result:";

    [SerializeField]
	private KeyCode keyPrint = KeyCode.Q;

    [SerializeField]
	private bool isPrintedOnDestroy = true;

    private void Update()
    {
        if (Input.GetKeyDown(keyPrint))
        {
            Print();
        }
    }

    private void OnDestroy()
    {
        if (isPrintedOnDestroy)
        {
            Print(messageByDefaultOnDestroy);
        }
    }

    public static void Print(string messagge)
    {
        DebugPrinter.Print(messagge + Count());
    }

    public static void Print()
    {
        DebugPrinter.Print(messageByDefault + Count());
    }

    public static int Count()
    {
        return Resources.FindObjectsOfTypeAll(typeof(Material)).Length;
    }
}