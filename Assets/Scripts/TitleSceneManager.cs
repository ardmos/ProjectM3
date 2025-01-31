using UnityEngine;

/// <summary>
/// 타이틀씬을 관리하는 매니저입니다.
/// 씬의 유일한 UI의 기능을 관리합니다.
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    public CustomClickSoundButton StartButton;

    private void Start()
    {
        StartButton.AddClickListener(()=>LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene));
    }

}
