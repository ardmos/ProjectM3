using UnityEngine;

/// <summary>
/// 게임씬의 UI를 관리하는 매니저 클래스입니다. 
/// 현재는 하나뿐인 버튼의 기능을 관리합니다.
/// </summary>
public class UIManager : MonoBehaviour
{
    public CustomClickSoundButton BackButton;

    private void Start()
    {
        BackButton.AddClickListener(() => { LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene); });
    }
}
