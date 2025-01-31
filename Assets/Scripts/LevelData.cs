using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 스테이지 씬에 존재하는 클래스입니다. 
/// 해당 씬의 규칙과 정보를 저장하는데 쓰입니다.
/// </summary>
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
