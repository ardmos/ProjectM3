using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSettingsPanelController : MonoBehaviour
{
    public Button CloseButton;
    public Slider MusicSlider;
    public Slider SoundSlider;

    private void Start()
    {
        // �����̴����� ����Ŵ��� ���� �� ��ɱ���. 

        CloseButton.onClick.AddListener(Hide);

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
