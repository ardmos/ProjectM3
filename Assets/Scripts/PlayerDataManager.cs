using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ������(Ŭ������ �������� ����)�� �����ϴ� Ŭ�����Դϴ�. 
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
    /// �÷��̾� �����͸� �ʱ�ȭ�ϴ� �޼����Դϴ�. 
    /// ������ ���̺�ý����� ���� �ε��ϴµ�, ������ ó�� �ϴ� ���ó�� �ε��� ������ ���� ��� ���� �����մϴ�.
    /// </summary>
    private void InitPlayerData()
    {
        PlayerData = SaveSystem.LoadData<PlayerData>();

        if (PlayerData == null)
        {
            Debug.Log($"�ε��� PlayerData ������ �����ϴ�. ���� �����մϴ�.");

            PlayerData = new PlayerData();

            if (PlayerData.StageScorePairs.Count == 0)
            {
                // ���� ù ���� ����
                PlayerData.StageScorePairs = new Dictionary<int, int>() { { 1, 0 } };
            }
        }
        else
        {
            Debug.Log($"PlayerData �ε� ����!");
        }
    }

    /// <summary>
    /// �������� Ŭ���� ������ �����ϴ� �޼����Դϴ�. ���� �� ���̺�ý����� ���� ���� ������ �����մϴ�.
    /// </summary>
    public void UpdatePlayerStageClearData(int stage, int starScore)
    {
        if (PlayerData == null)
        {
            Debug.LogError("PlayerData�� �����ϴ�.");
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
