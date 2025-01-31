using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 데이터(클리어한 스테이지 정보)를 관리하는 클래스입니다. 
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public PlayerData PlayerData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitPlayerData();
    }

    /// <summary>
    /// 플레이어 데이터를 초기화하는 메서드입니다. 
    /// 보통은 세이브시스템을 통해 로드하는데, 게임을 처음 하는 경우처럼 로드할 정보가 없는 경우 새로 생성합니다.
    /// </summary>
    private void InitPlayerData()
    {
        PlayerData = SaveSystem.LoadData<PlayerData>();

        if (PlayerData == null)
        {
            Debug.Log($"로드할 PlayerData 정보가 없습니다. 새로 생성합니다.");

            PlayerData = new PlayerData();

            if (PlayerData.StageScorePairs.Count == 0)
            {
                // 게임 첫 도전 상태
                PlayerData.StageScorePairs = new Dictionary<int, int>() { { 1, 0 } };
            }
        }
        else
        {
            Debug.Log($"PlayerData 로드 성공!");
        }
    }

    /// <summary>
    /// 스테이지 클리어 정보를 갱신하는 메서드입니다. 갱신 후 세이브시스템을 통해 갱신 내용을 저장합니다.
    /// </summary>
    public void UpdatePlayerStageClearData(int stage, int starScore)
    {
        if (PlayerData == null)
        {
            Debug.LogError("PlayerData가 없습니다.");
            return;
        }

        if(PlayerData.StageScorePairs.ContainsKey(stage)){
            PlayerData.StageScorePairs[stage] = starScore;
        }
        else
        {
            PlayerData.StageScorePairs.Add(stage, starScore);
        }

        SaveSystem.SavePlayerData(PlayerData);
    }

    public PlayerData GetPlayerData() => PlayerData;
}
