using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static LevelData Instance;

    public int Level;
    [Header("클리어 미션 목록")]
    public List<TargetData> TargetList = new();
    [Header("단계별 스타 점수")]
    public int[] TargetScore = new int[3];
    [Header("가능한 무브 횟수")]
    public int MoveMaxCount;
    [Header("이번 스테이지에 등장시킬 캔디 목록")]
    public Candy[] CandyPrefabs;

    private void Awake()
    {
        Instance = this;
    }
}
