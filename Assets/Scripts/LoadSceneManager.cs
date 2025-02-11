using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 화면 전환을 관리하는 매니저입니다.
/// </summary>
public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance;

    public enum Scene
    {
        TitleScene,
        StageSelectScene,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Max
    }

    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 새로운 화면으로 전환합니다. 페이드 효과를 사용합니다.
    /// </summary>
    public void Load(Scene targetScene)
    {
        try
        {
            StartCoroutine(FadeAndLoadScene(targetScene));
        }
        catch (Exception e)
        {
            Debug.LogError($"씬 로드중 문제가 발생했습니다. {e.Message}");
            Debug.LogException(e);
            AndroidToast.ShowToast(e.Message);
        }
    }

    /// <summary>
    /// 배너광고 표시상태를 조절하는 메서드입니다
    /// </summary>
    /// <param name="targetScene"></param>
    private void BannerAdHandler(Scene targetScene)
    {
        switch (targetScene)
        {
            case Scene.TitleScene:
            case Scene.StageSelectScene:
                break;
            default:
                // 게임씬
                BannerAdManager.Instance.ShowBannerAd();
                break;
        }      
    }

    /// <summary>
    /// 로드되는 씬에 맞춰 BGM을 재생해주는 메서드입니다
    /// </summary>
    private void PlayBGM(Scene targetScene)
    {
        switch (targetScene)
        {
            case Scene.TitleScene:
                SoundManager.Instance.PlayBGM(SoundManager.BGM.TitleScene);
                break;
            case Scene.StageSelectScene:
                SoundManager.Instance.PlayBGM(SoundManager.BGM.StageSelectScene);
                break;
            default:
                // 게임씬
                SoundManager.Instance.PlayBGM(SoundManager.BGM.GameScene);
                break;
        }
    }

    /// <summary>
    /// 페이드 효과를 재생하고 씬을 로드해주는 메서드입니다.
    /// </summary>
    private IEnumerator FadeAndLoadScene(Scene targetScene)
    {
        // 페이드 효과를 위한 배너광고 비활성화
        BannerAdManager.Instance.HideBannerAd();

        // 페이드 아웃
        yield return StartCoroutine(Fade(1f));

        // 씬 로드
        SceneManager.LoadScene(targetScene.ToString());

        // 페이드 인
        yield return StartCoroutine(Fade(0f));

        // 배경음 설정
        PlayBGM(targetScene);
        // 배너 광고 설정
        BannerAdHandler(targetScene);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }
}
