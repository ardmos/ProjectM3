using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스테이지 선택 화면에 표시되는 스테이지 패널입니다.
/// 각 스테이지의 정보를 유저에게 표시해줍니다.
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

        // 클릭시 스테이지 레벨의 상세 정보를 나타내는 팝업을 노출시킵니다
        StageButton.AddClickListener(() => {
            StageSelectSceneManager.Instance.PopupLevelPanelController.Show();
            StageSelectSceneManager.Instance.PopupLevelPanelController.InitPopup(stageLevel, stageName, starScore);
        });
    }
}
