using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

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
