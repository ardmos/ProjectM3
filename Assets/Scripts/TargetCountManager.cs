using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지씬의 타겟 시스템을 관리하는 매니저 클래스입니다.
/// 타겟 생성과 각 타겟 클리어 처리, 모든 타겟 클리어시 게임 승리 처리를 담당합니다. 
/// </summary>
public class TargetCountManager : MonoBehaviour
{
    public RectTransform TargetGrid;
    public Target TargetPrefab;

    [SerializeField] private Sprite[] candySprites;
    [SerializeField] private List<Target> targets;

    void Start()
    {
        InitTargetPanel();
    }

    /// <summary>
    /// 현재 씬 기획에 따라 타겟 생성
    /// </summary>
    private void InitTargetPanel()
    {
        foreach (TargetData target in LevelData.Instance.TargetList)
        {
            Target targetObject = Instantiate(TargetPrefab, TargetGrid);
            targetObject.InitTarget(candySprites[(int)target.CandyType], target.CandyType, target.Count);
            targets.Add(targetObject);
        }
    }

    /// <summary>
    /// 타겟 클리어 현황을 확인해주는 메서드입니다.
    /// 타겟이 클리어상태가 아니면 타겟 카운트를 감소시킵니다. 
    /// 모든 타겟이 클리어 상태면 GameManager에게 스테이지 승리 요청을 보냅니다.
    /// </summary>
    public void CheckTargetClear(List<Candy> poppedCandies)
    {
        foreach (Candy candy in poppedCandies)
        {
            foreach (Target target in targets)
            {
                if (!target.Clear && target.CandyType == candy.CandyType)
                {
                    target.DecreaseTargetCount();
                }
            }
        }

        bool allTargetsClear = true;
        foreach (Target target in targets)
        {
            if (!target.Clear)
            {
                allTargetsClear = false;
            }
        }

        if (allTargetsClear && GameManager.Instance.GetGameState() != GameManager.State.Win)
        {
            StartCoroutine(StageClear());
        }
    }

    private IEnumerator StageClear()
    {
        yield return new WaitForSeconds(1.5f);
        // 스테이지 클리어!
        GameManager.Instance.UpdateState(GameManager.State.Win);
    }
}
