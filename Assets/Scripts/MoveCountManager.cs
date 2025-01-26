using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveCountManager : MonoBehaviour
{
    private const int MAX_COUNT = 10;

    public TextMeshProUGUI moveCountText;
    public int moveCount;

    private void Start()
    {
        moveCountText.text = $"{moveCount}/{MAX_COUNT}";
    }

    public void IncreaseMoveCount()
    {
        moveCount++;
        moveCountText.text = $"{moveCount}/{MAX_COUNT}";

        if(moveCount >= MAX_COUNT)
        {
            GameManager.Instance.SetGameState(GameManager.State.Lose);
        }
    }
}
