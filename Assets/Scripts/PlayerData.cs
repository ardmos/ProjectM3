using System.Collections.Generic;

/// <summary>
/// 세이브 시스템에 쓰일 플레이어 데이터 클래스입니다.
/// 플레이어가 클리어한 스테이지 정보를 관리합니다.
/// </summary>
[System.Serializable]
public class PlayerData
{
    public Dictionary<int, int> StageScorePairs = new();
}
