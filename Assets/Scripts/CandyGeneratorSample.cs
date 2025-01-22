using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween ���ӽ����̽� �߰�

public class CandyGeneratorSample : MonoBehaviour
{
    public GameObject candyPrefab;
    public GameBoardManagerSample gameBoardManager;
    public RectTransform areaCandyGenerate;
    public float dropDuration = 0.5f; // ĵ�� �������� �ð�

    private Queue<(int column, int count)> candyQueue = new Queue<(int column, int count)>();

    private void Awake()
    {
        gameBoardManager.OnNeedCandy += GameBoardManager_OnNeedCandy;
    }

    private void Update()
    {
        if (candyQueue.Count > 0)
        {
            GenerateCandy(candyQueue.Dequeue());
        }
    }

    private void GameBoardManager_OnNeedCandy(object sender, (int column, int count) e)
    {
        candyQueue.Enqueue(e);
    }

    private void GenerateCandy((int column, int count) candy)
    {
        float xPosition = (candy.column - 5) * 100f;

        for (int i = 0; i < candy.count; i++)
        {
            float yPosition = i * 100f;
            Vector2 startPosition = new Vector2(xPosition, areaCandyGenerate.rect.height);
            Vector2 endPosition = new Vector2(xPosition, yPosition);

            GameObject newCandy = Instantiate(candyPrefab, areaCandyGenerate);
            RectTransform rectTransform = newCandy.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = startPosition;

                // DoTween�� ����� �ִϸ��̼�
                rectTransform.DOAnchorPos(endPosition, dropDuration)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        // �ִϸ��̼� �Ϸ� �� ���� ���� ������Ʈ
                        gameBoardManager.UpdateCandyArray(candy.column, candy.count - i - 1);
                    });
            }
        }
    }
}
