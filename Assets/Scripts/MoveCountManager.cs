using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// ������ �÷��̾��� ���� ī��Ʈ�� �����ϴ� Ŭ�����Դϴ�.
/// ���� ī��Ʈ�� �ִ�ġ�� �����ϸ� ������ �й�ó�� ��ŵ�ϴ�.
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
    /// ����ī��Ʈ ����
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
    /// UI ������Ʈ
    /// </summary>
    public void UpdateMoveCountTextUI(int moveCount)
    {
        MoveCountText.text = $"{moveCount}/{LevelData.Instance.MoveMaxCount}";
    }

    /// <summary>
    /// �������� �й�� ���� �Ŵ����� �����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReportStageLoseWithDelay()
    {
        yield return new WaitForSeconds(1f);

        GameManager.Instance.UpdateState(GameManager.State.Lose);
    }
}
