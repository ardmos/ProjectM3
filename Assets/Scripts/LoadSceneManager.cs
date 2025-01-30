using UnityEngine.SceneManagement;

public static class LoadSceneManager 
{
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

    public static void Load(Scene targetScene)
    {
        SceneManager.LoadScene(targetScene.ToString());

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
}
