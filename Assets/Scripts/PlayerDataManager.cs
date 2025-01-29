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
        PlayerData = new PlayerData();

        if (PlayerData == null) return;

        if(PlayerData.StageScorePairs.Count == 0)
        {
            // 게임 첫 도전 상태
            PlayerData.StageScorePairs = new Dictionary<int, int>() { {1, 0} };
        }
    }
}
