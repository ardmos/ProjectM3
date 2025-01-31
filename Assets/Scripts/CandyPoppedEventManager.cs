using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĵ�� ���� �� ȣ��Ǵ� OnPopped �̺�Ʈ�� ���� Ŭ�����Դϴ�. 
/// �̺�Ʈ �ڵ鷯���� ���� ������ �����մϴ�.
/// </summary>
public class CandyPoppedEventManager : MonoBehaviour
{
    public ScoreManager ScoreManager;
    public TargetCountManager TargetCountManager;

    private void Start()
    {
        GameBoardManager.Instance.OnPopped += HandlePoppedEvent;
    }

    private void HandlePoppedEvent(List<Candy> poppedCandies)
    {
        // ���� ScoreManager�� AddScore ����
        ScoreManager.AddScore(poppedCandies);

        // �� ���� TargetCountManager�� CheckTargetClear ����
        TargetCountManager.CheckTargetClear(poppedCandies);
    }
}
