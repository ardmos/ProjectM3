using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스테이지의 클리어 목표인 타겟 클래스입니다. 
/// 각 타겟의 정보를 관리합니다.
/// </summary>
public class Target : MonoBehaviour
{
    public Image TargetCandyImage;
    public Image TargetClearImage;
    public TextMeshProUGUI TargetCountText;
    public CandyType CandyType;
    public int TargetCount;
    public bool Clear = false;

    public void InitTarget(Sprite targetCandyImage, CandyType candyType, int targetCount)
    {
        TargetCandyImage.sprite = targetCandyImage;
        TargetClearImage.enabled = false;
        TargetCountText.text = targetCount.ToString();
        CandyType = candyType;
        TargetCount = targetCount;
    }

    /// <summary>
    /// 플레이어가 타겟을 달성했을 시 호출되는 메서드로, 호출시마다 타겟카운트를 감소시키다 모두 감소하면 현 타겟을 클리어처리 합니다.
    /// 모든 타겟이 클리어 처리되면 TargetCountManager에서 스테이지를 승리 처리합니다.
    /// </summary>
    public void DecreaseTargetCount()
    {
        --TargetCount;

        TargetCountText.text = TargetCount.ToString();

        if (TargetCount <= 0)
        {
            TargetClearImage.enabled = true;
            Clear = true;
        }
    }
}
