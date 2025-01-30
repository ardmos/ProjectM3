using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
/// <summary>
/// Ŭ���̾�Ʈ�� ������ ��� ������ �����ϱ� ������ ����ϴ� ��ũ��Ʈ�Դϴ�.
/// ���ÿ� Ŭ���̾�Ʈ�� ������ �������ݴϴ�.
/// </summary>
public static class SaveSystem
{
    // candy�� ���̺� ���� �ڷ���.
    private static string path = Application.persistentDataPath + $"/playerSaveData.candy";

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
                Debug.LogError($"�÷��̾� �����͸� ���̺��ϴµ� ������ �߻��߽��ϴ�: {e.Message}");
            }
        }
    }

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
                Debug.LogError($"���� ���� �����͸� ���̺��ϴµ� ������ �߻��߽��ϴ�: {e.Message}");
            }
        }
    }

    public static T LoadData<T>() where T : class
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"���̺������� �߰����� ���߽��ϴ�: {path}");
            return null;
        }

        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            if (fileStream.Length <= 0)
            {
                Debug.LogError("���̺� ������ �ջ�ƽ��ϴ�. ������ �����մϴ�");
                fileStream.Close();
                File.Delete(path);
                if (!File.Exists(path))
                    Debug.LogError("���� ����");
                else
                    Debug.LogError("���� ����");
                return null;
            }

            // ���̺����� �ε�
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
                    Debug.LogError($"�������� �ʴ� ������ Ÿ���Դϴ�.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"���̺������� �о���µ� ������ �߻��߽��ϴ�: {e.Message}");
                return null;
            }
        }

    }
}