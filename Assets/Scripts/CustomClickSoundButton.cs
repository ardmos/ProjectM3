using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI 버튼 클릭 사운드 추가를 위한 커스텀 버튼 컴포넌트입니다.
/// </summary>
public class CustomClickSoundButton : Button
{
    public void AddClickListener(UnityAction call)
    {
        onClick.AddListener(() => {
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Button);
            call();
        });
    }
}
