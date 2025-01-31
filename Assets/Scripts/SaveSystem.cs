using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// 게임 데이터를 로컬에 저장하고 로드해주는 클래스입니다.
/// </summary>
public static class SaveSystem
{
    // candy는 세이브 파일 자료형.
    private static string path = Application.persistentDataPath + $"/playerSaveData.candy";

    /// <summary>
    /// 플레이어 데이터 저장
    /// </summary>
    /// <param name="playerData"></param>
    public static void SavePlayerData(PlayerData playerData)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SaveData saveData = new SaveData(playerData, SoundManager.Instance.GetSoundVolumeData());
                binaryFormatter.Serialize(fileStream, saveData);
            }
            catch (Exception e)
            {
                Debug.LogError($"플레이어 데이터를 세이브하는데 문제가 발생했습니다: {e.Message}");
            }
        }
    }

    /// <summary>
    /// 사운드 데이터 저장
    /// </summary>
    /// <param name="soundVolumeData"></param>
    public static void SaveSoundVolumeData(SoundVolumeData soundVolumeData)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SaveData saveData = new SaveData(PlayerDataManager.Instance.GetPlayerData(), soundVolumeData);
                binaryFormatter.Serialize(fileStream, saveData);
            }
            catch (Exception e)
            {
                Debug.LogError($"사운드 설정 데이터를 세이브하는데 문제가 발생했습니다: {e.Message}");
            }
        }
    }

    /// <summary>
    /// 데이터 로드시에 사용하는 메서드입니다. 
    /// 세이브 파일이 손상되었을 경우 자동으로 파일을 삭제시킵니다.
    /// </summary>
    public static T LoadData<T>() where T : class
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"세이브파일을 발견하지 못했습니다: {path}");
            return null;
        }

        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            if (fileStream.Length <= 0)
            {
                Debug.LogError("세이브 파일이 손상됐습니다. 파일을 삭제합니다");
                fileStream.Close();
                File.Delete(path);
                if (!File.Exists(path))
                    Debug.LogError("삭제 성공");
                else
                    Debug.LogError("삭제 실패");
                return null;
            }

            // 세이브파일 로드
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SaveData saveData = binaryFormatter.Deserialize(fileStream) as SaveData;

                if (typeof(T) == typeof(SoundVolumeData))
                {
                    return saveData.soundVolumeData as T;
                }
                else if (typeof(T) == typeof(PlayerData))
                {
                    return saveData.playerData as T;
                }
                else
                {
                    Debug.LogError($"지원하지 않는 데이터 타입입니다.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"세이브파일을 읽어오는데 문제가 발생했습니다: {e.Message}");
                return null;
            }
        }

    }
}