using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������ Ÿ�� �ý����� �����ϴ� �Ŵ��� Ŭ�����Դϴ�.
/// Ÿ�� ������ �� Ÿ�� Ŭ���� ó��, ��� Ÿ�� Ŭ����� ���� �¸� ó���� ����մϴ�. 
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
    /// ���� �� ��ȹ�� ���� Ÿ�� ����
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
    /// Ÿ�� Ŭ���� ��Ȳ�� Ȯ�����ִ� �޼����Դϴ�.
    /// Ÿ���� Ŭ������°� �ƴϸ� Ÿ�� ī��Ʈ�� ���ҽ�ŵ�ϴ�. 
    /// ��� Ÿ���� Ŭ���� ���¸� GameManager���� �������� �¸� ��û�� �����ϴ�.
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
        // �������� Ŭ����!
        GameManager.Instance.UpdateState(GameManager.State.Win);
    }
}
