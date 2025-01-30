using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        Level5
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

    public void Load(Scene targetScene)
    {
        StartCoroutine(FadeAndLoadScene(targetScene.ToString()));

        PlayBGM(targetScene);
    }

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
                // ∞‘¿”æ¿
                SoundManager.Instance.PlayBGM(SoundManager.BGM.GameScene);
                break;
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // ∆‰¿ÃµÂ æ∆øÙ
        yield return StartCoroutine(Fade(1f));

        // æ¿ ∑ŒµÂ
        SceneManager.LoadScene(sceneName);

        // ∆‰¿ÃµÂ ¿Œ
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
