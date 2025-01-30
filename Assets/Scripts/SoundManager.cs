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
    }

    private void Start()
    {
        PlayBGM(BGM.TitleScene);
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
}
