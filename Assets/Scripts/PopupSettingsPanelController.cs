using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
