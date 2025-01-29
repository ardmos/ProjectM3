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
    }
}
