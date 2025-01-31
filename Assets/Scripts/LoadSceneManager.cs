using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ȭ�� ��ȯ�� �����ϴ� �Ŵ����Դϴ�.
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
    /// ���ο� ȭ������ ��ȯ�մϴ�. ���̵� ȿ���� ����մϴ�.
    /// </summary>
    public void Load(Scene targetScene)
    {
        StartCoroutine(FadeAndLoadScene(targetScene.ToString()));

        PlayBGM(targetScene);
        BannerAdHandler(targetScene);
    }

    /// <summary>
    /// ��ʱ��� ǥ�û��¸� �����ϴ� �޼����Դϴ�
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
                // ���Ӿ�
                BannerAdManager.Instance.ShowBannerAd();
                break;
        }      
    }

    /// <summary>
    /// �ε�Ǵ� ���� ���� BGM�� ������ִ� �޼����Դϴ�
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
                // ���Ӿ�
                SoundManager.Instance.PlayBGM(SoundManager.BGM.GameScene);
                break;
        }
    }

    /// <summary>
    /// ���̵� ȿ���� ������ִ� �޼����Դϴ�.
    /// </summary>
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(1f));

        // �� �ε�
        SceneManager.LoadScene(sceneName);

        // ���̵� ��
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
