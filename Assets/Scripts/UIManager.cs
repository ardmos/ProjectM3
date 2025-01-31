using UnityEngine;

/// <summary>
/// ���Ӿ��� UI�� �����ϴ� �Ŵ��� Ŭ�����Դϴ�. 
/// ����� �ϳ����� ��ư�� ����� �����մϴ�.
/// </summary>
public class UIManager : MonoBehaviour
{
    public CustomClickSoundButton BackButton;

    private void Start()
    {
        BackButton.AddClickListener(() => { LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene); });
    }
}
