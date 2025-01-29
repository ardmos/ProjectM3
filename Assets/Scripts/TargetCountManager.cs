using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class TargetCountManager : MonoBehaviour
{
    public RectTransform TargetGrid;
    public Target TargetPrefab;

    [SerializeField] private Sprite[] candySprites;
    [SerializeField] private List<Target> targets;

    void Start()
    {
        GameBoardManager.Instance.OnPopped += Instance_OnPopped;

        InitTargetPanel();
    }

    private void InitTargetPanel()
    {
        foreach (TargetData target in LevelData.Instance.TargetList)
        {
            Target targetObject = Instantiate(TargetPrefab, TargetGrid);
            targetObject.InitTarget(candySprites[(int)target.CandyType], target.CandyType, target.Count);
            targets.Add(targetObject);
        }
    }

    private void Instance_OnPopped(List<Candy> poppedCandies)
    {
        bool allTargetsClear = true;

        foreach (Candy candy in poppedCandies)
        {
            foreach (Target target in targets)
            {
                if (!target.Clear && target.CandyType == candy.CandyType)
                {
                    target.DecreaseTargetCount();
                    allTargetsClear &= target.Clear; // 현재 타겟이 클리어된 경우를 체크
                }
            }
        }

        if (allTargetsClear)
        {
            // 스테이지 클리어!
            GameManager.Instance.UpdateState(GameManager.State.Win);
        }
    }
}
