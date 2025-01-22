using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween 네임스페이스 추가

public class CandyGenerator : MonoBehaviour
{
    public GameObject candyPrefab;
    public GameBoardManager gameBoardManager;
    public RectTransform areaCandyGenerate;
    public float dropDuration = 0.5f; // 캔디가 떨어지는 시간

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

    }
}
