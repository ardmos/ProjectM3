using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveCountManager : MonoBehaviour
{
    public TextMeshProUGUI moveCountText;
    public int moveCount;

    public void IncreaseMoveCount()
    {
        moveCount++;
        moveCountText.text = $"{moveCount}";
    }
}
