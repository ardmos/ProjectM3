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
        StartCoroutine(FadeAndLoadScene(targetScene.ToString()));

        PlayBGM(targetScene);
        BannerAdHandler(targetScene);
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
                BannerAdManager.Instance.HideBannerAd();
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
    /// 페이드 효과를 재생해주는 메서드입니다.
    /// </summary>
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 페이드 아웃
        yield return StartCoroutine(Fade(1f));

        // 씬 로드
        SceneManager.LoadScene(sceneName);

        // 페이드 인
        yield return StartCoroutine(Fade(0f));
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
