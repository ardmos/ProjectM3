using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveCountManager : MonoBehaviour
{
    public TextMeshProUGUI MoveCountText;
    public int MoveCount;

    private void Start()
    {
        UpdateMoveCountTextUI(MoveCount);

        GameBoardManager.Instance.OnMoved += IncreaseMoveCount;
    }

    private void IncreaseMoveCount()
    {
        MoveCount++;
        UpdateMoveCountTextUI(MoveCount);

        if (MoveCount >= LevelData.Instance.MoveMaxCount)
        {
            StartCoroutine(StageLose());
        }
    }

    public void UpdateMoveCountTextUI(int moveCount)
    {
        MoveCountText.text = $"{moveCount}/{LevelData.Instance.MoveMaxCount}";
    }

    private IEnumerator StageLose()
    {
        yield return new WaitForSeconds(1f);
        // 스테이지 실패!
        GameManager.Instance.UpdateState(GameManager.State.Lose);
    }
}
