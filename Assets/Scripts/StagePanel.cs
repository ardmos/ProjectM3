using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������� ���� ȭ�鿡 ǥ�õǴ� �������� �г��Դϴ�.
/// �� ���������� ������ �������� ǥ�����ݴϴ�.
/// </summary>
public class StagePanel : MonoBehaviour
{
    public TextMeshProUGUI StageLevelTextUI;
    public Image StageImageUI;
    public TextMeshProUGUI StageNameTextUI;
    public GameObject[] StarObjects = new GameObject[3];
    public CustomClickSoundButton StageButton;
    public Sprite[] BGSprites = new Sprite[2];
    public Image BGImageUI;

    public void InitPanel(int stageLevel, Sprite stageImage, string stageName, int starScore)
    {
        StageLevelTextUI.text = stageLevel.ToString();
        StageImageUI.sprite = stageImage;
        StageNameTextUI.text = stageName;

        for (int i = 0; i < StarObjects.Length; i++)
        {
            StarObjects[i].SetActive(i < starScore);
        }

        BGImageUI.sprite = BGSprites[stageLevel % 2];

        // Ŭ���� �������� ������ �� ������ ��Ÿ���� �˾��� �����ŵ�ϴ�
        StageButton.AddClickListener(() => {
            StageSelectSceneManager.Instance.PopupLevelPanelController.Show();
            StageSelectSceneManager.Instance.PopupLevelPanelController.InitPopup(stageLevel, stageName, starScore);
        });
    }
}
