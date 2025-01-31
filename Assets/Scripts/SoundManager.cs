using System.Collections;
using UnityEngine;

/// <summary>
/// ���� ���� ������ �����ϴ� ���� �Ŵ����Դϴ�.
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
    /// ���� ���� �����͸� �ʱ�ȭ�ϴ� �޼����Դϴ�. 
    /// ������ ���̺�ý����� ���� �ε��ϴµ�, ������ ó�� �ϴ� ���ó�� �ε��� ������ ���� ��� ���� �����մϴ�.
    /// </summary>
    private void InitSoundVolumeData()
    {
        var soundVolumeData = SaveSystem.LoadData<SoundVolumeData>();

        if (soundVolumeData == null)
        {
            Debug.Log($"�ε��� soundVolumeData ������ �����ϴ�. �⺻ �� ����");
        }
        else
        {
            Debug.Log($"PlayerData �ε� ����!");
            audioSourceBGM.volume = soundVolumeData.BGMVolume;
            audioSourceSFX.volume = soundVolumeData.SFXVolume;
        }
    }

    /// <summary>
    /// ������� ���
    /// </summary>
    public void PlayBGM(BGM bgmName)
    {
        audioSourceBGM.Stop();
        audioSourceBGM.clip = BGMClips[(int)bgmName];
        audioSourceBGM.Play();
    }

    /// <summary>
    /// ȿ���� ���
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
    /// Ư���� �Ҹ��� ū GenerateCandySFX ������ ����ɶ����� ������ ��� �����ִ� �޼����Դϴ�.
    /// ����� ����Ǹ� �ٽ� ���� �������� �����մϴ�.
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
    /// ����� ���� ������Ʈ
    /// </summary>
    public void UpdateBGMVolume(float volume)
    {
        audioSourceBGM.volume = volume;
        SaveSystem.SaveSoundVolumeData(new SoundVolumeData(GetBGMVolume(), GetSFXVolume()));
    }

    /// <summary>
    /// ȿ���� ���� ������Ʈ
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
