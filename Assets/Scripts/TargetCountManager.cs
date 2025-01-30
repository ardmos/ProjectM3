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

        if (allTargetsClear)
        {
            StartCoroutine(StageClear());
        }
    }

    private IEnumerator StageClear()
    {
        yield return new WaitForSeconds(1f);
        // 스테이지 클리어!
        GameManager.Instance.UpdateState(GameManager.State.Win);
    }
}
