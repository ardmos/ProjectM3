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
        // 슬라이더들은 사운드매니저 구현 후 기능구현. 

        CloseButton.AddClickListener(Hide);

        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
