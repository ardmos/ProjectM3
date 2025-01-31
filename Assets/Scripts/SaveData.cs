/// <summary>
/// 세이브시스템에서 사용하는 세이브 데이터 클래스입니다.
/// </summary>
[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public SoundVolumeData soundVolumeData;

    public SaveData(PlayerData playerData, SoundVolumeData soundVolumeData)
    {
        this.playerData = playerData;
        this.soundVolumeData = soundVolumeData;
    }
}
