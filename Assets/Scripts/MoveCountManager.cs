using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 게임중 플레이어의 무브 카운트를 관리하는 클래스입니다.
/// 무브 카운트가 최대치에 도달하면 게임을 패배처리 시킵니다.
/// </summary>
public class MoveCountManager : MonoBehaviour
{
    public TextMeshProUGUI MoveCountText;
    public int MoveCount;

    private void Start()
    {
        UpdateMoveCountTextUI(MoveCount);

        GameBoardManager.Instance.OnMoved += IncreaseMoveCount;
    }

    /// <summary>
    /// 무브카운트 증가
    /// </summary>
    private void IncreaseMoveCount()
    {
        MoveCount++;
        UpdateMoveCountTextUI(MoveCount);

        if (MoveCount >= LevelData.Instance.MoveMaxCount)
        {
            StartCoroutine(ReportStageLoseWithDelay());
        }
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    public void UpdateMoveCountTextUI(int moveCount)
    {
        MoveCountText.text = $"{moveCount}/{LevelData.Instance.MoveMaxCount}";
    }

    /// <summary>
    /// 스테이지 패배시 게임 매니저에 보고하는 메서드입니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReportStageLoseWithDelay()
    {
        yield return new WaitForSeconds(1f);

        GameManager.Instance.UpdateState(GameManager.State.Lose);
    }
}
