using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스테이지 선택씬에 있는 설정팝업입니다.
/// 사운드 관련 설정을 조정할 수 있도록 합니다.
/// </summary>
public class PopupSettingsPanelController : MonoBehaviour
{
    public CustomClickSoundButton CloseButton;
    public Slider MusicSlider;
    public Slider SoundSlider;

    private void Start()
    {
        MusicSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.UpdateBGMVolume(MusicSlider.value); });
        SoundSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.UpdateSFXVolume(SoundSlider.value); });
        CloseButton.AddClickListener(Hide);

        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        MusicSlider.value = SoundManager.Instance.GetBGMVolume();
        SoundSlider.value = SoundManager.Instance.GetSFXVolume();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
