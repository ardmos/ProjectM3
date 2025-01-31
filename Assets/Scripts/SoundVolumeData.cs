/// <summary>
/// 세이브 시스템에서 사용하는 사운드 설정 정보 클래스입니다.
/// </summary>
[System.Serializable]
public class SoundVolumeData
{
    public float BGMVolume;
    public float SFXVolume;

    public SoundVolumeData(float bgmVolume, float sfxVolume)
    {
        this.BGMVolume = bgmVolume;
        this.SFXVolume = sfxVolume;
    }

    public void UpdateData(float bgmVolume, float sfxVolume)
    {
        this.BGMVolume = bgmVolume;
        this.SFXVolume = sfxVolume;
    }
}