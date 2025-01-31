using System.Collections;
using UnityEngine;

/// <summary>
/// 사운드 관련 로직을 관리하는 사운드 매니저입니다.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public enum BGM
    {
        TitleScene,
        StageSelectScene,
        GameScene
    }

    public enum SFX
    {
        GenerateCandy,
        Win,
        Lose,
        Pop,
        Swap,
        Button
    }

    public AudioSource audioSourceBGM;
    public AudioSource audioSourceSFX;

    public AudioClip[] BGMClips = new AudioClip[3];
    public AudioClip[] SFXClips = new AudioClip[6];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        InitSoundVolumeData();
    }

    private void Start()
    {
        PlayBGM(BGM.TitleScene);
    }

    /// <summary>
    /// 사운드 설정 데이터를 초기화하는 메서드입니다. 
    /// 보통은 세이브시스템을 통해 로드하는데, 게임을 처음 하는 경우처럼 로드할 정보가 없는 경우 새로 생성합니다.
    /// </summary>
    private void InitSoundVolumeData()
    {
        var soundVolumeData = SaveSystem.LoadData<SoundVolumeData>();

        if (soundVolumeData == null)
        {
            Debug.Log($"로드할 soundVolumeData 정보가 없습니다. 기본 값 적용");
        }
        else
        {
            Debug.Log($"PlayerData 로드 성공!");
            audioSourceBGM.volume = soundVolumeData.BGMVolume;
            audioSourceSFX.volume = soundVolumeData.SFXVolume;
        }
    }

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    public void PlayBGM(BGM bgmName)
    {
        audioSourceBGM.Stop();
        audioSourceBGM.clip = BGMClips[(int)bgmName];
        audioSourceBGM.Play();
    }

    /// <summary>
    /// 효과음 재생
    /// </summary>
    public void PlaySFX(SFX sfxName)
    {
        audioSourceSFX.Stop();
        audioSourceSFX.clip = SFXClips[(int)sfxName];

        if(sfxName == SFX.GenerateCandy)
            StartCoroutine(GenerateCandySFXVolumeControl());
        else
            audioSourceSFX.Play();
    }

    /// <summary>
    /// 특별히 소리가 큰 GenerateCandySFX 에셋이 재생될때에만 볼륨을 잠시 낮춰주는 메서드입니다.
    /// 재생이 종료되면 다시 원래 볼륨으로 복구합니다.
    /// </summary>
    private IEnumerator GenerateCandySFXVolumeControl()
    {
        float originalVolume = audioSourceSFX.volume;
        audioSourceSFX.volume *= 0.5f;
        audioSourceSFX.Play();
        yield return new WaitForSeconds(audioSourceSFX.clip.length);
        audioSourceSFX.volume = originalVolume;
    }


    /// <summary>
    /// 배경음 볼륨 업데이트
    /// </summary>
    public void UpdateBGMVolume(float volume)
    {
        audioSourceBGM.volume = volume;
        SaveSystem.SaveSoundVolumeData(new SoundVolumeData(GetBGMVolume(), GetSFXVolume()));
    }

    /// <summary>
    /// 효과음 볼륨 업데이트
    /// </summary>
    public void UpdateSFXVolume(float volume)
    {
        audioSourceSFX.volume = volume;
        SaveSystem.SaveSoundVolumeData(new SoundVolumeData(GetBGMVolume(), GetSFXVolume()));
    }

    public float GetBGMVolume() { return audioSourceBGM.volume; }
    public float GetSFXVolume() { return audioSourceSFX.volume; }
    public SoundVolumeData GetSoundVolumeData()
    {
        return new SoundVolumeData(GetBGMVolume(), GetSFXVolume());
    }
}
