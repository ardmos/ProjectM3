using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayBGM(BGM bgmName)
    {
        audioSourceBGM.Stop();
        audioSourceBGM.clip = BGMClips[(int)bgmName];
        audioSourceBGM.Play();
    }

    public void PlaySFX(SFX sfxName)
    {
        audioSourceSFX.Stop();
        audioSourceSFX.clip = SFXClips[(int)sfxName];

        if(sfxName == SFX.GenerateCandy)
            StartCoroutine(GenerateCandySFXVolumeControl());
        else
            audioSourceSFX.Play();
    }

    private IEnumerator GenerateCandySFXVolumeControl()
    {
        float originalVolume = audioSourceSFX.volume;
        audioSourceSFX.volume *= 0.7f;
        audioSourceSFX.Play();
        yield return new WaitForSeconds(audioSourceSFX.clip.length);
        audioSourceSFX.volume = originalVolume;
    }

    public void UpdateBGMVolume(float volume)
    {
        audioSourceBGM.volume = volume;
        SaveSystem.SaveSoundVolumeData(new SoundVolumeData(GetBGMVolume(), GetSFXVolume()));
    }

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
