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

using TMPro;

public class ScoreManagerXR : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private static int score;

    private static bool isScoreChanged = false;

    private void Awake()
    {
        ResetScore();
    }

    public static void Increase(int scoreValue)
    {
        score += scoreValue;

        isScoreChanged = true;
    }

    public static int GetScore()
    {
        return score;
    }

    public static void ResetScore()
    {
        score = 0;

        isScoreChanged = true;
    }

    private void Update()
    {
        if (isScoreChanged)
        {
            text.text = score.ToString();

            isScoreChanged = false;
        }
    }

}