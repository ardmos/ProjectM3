using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text targetScoreText;
    public Text moveCountText;
    public Text currentScoreText;

    public void SetTargetScore(int score)
    {
        targetScoreText.text = "Target: " + score.ToString();
    }
    public void SetMoveCount(int count)
    {
        moveCountText.text = "Moves: " + count.ToString();
    }
    public void SetCurrentScore(int score)
    {
        currentScoreText.text = "Score: " + score.ToString();
    }
}
