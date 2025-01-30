using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public PlayerData GetPlayerData() => PlayerData;    
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
}
