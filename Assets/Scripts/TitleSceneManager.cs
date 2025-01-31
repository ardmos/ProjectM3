using UnityEngine;

/// <summary>
/// Ÿ��Ʋ���� �����ϴ� �Ŵ����Դϴ�.
/// ���� ������ UI�� ����� �����մϴ�.
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    public CustomClickSoundButton StartButton;

    private void Start()
    {
        StartButton.AddClickListener(()=>LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene));
    }

}
