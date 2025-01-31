using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������� Ŭ���� ��ǥ�� Ÿ�� Ŭ�����Դϴ�. 
/// �� Ÿ���� ������ �����մϴ�.
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
    /// �÷��̾ Ÿ���� �޼����� �� ȣ��Ǵ� �޼����, ȣ��ø��� Ÿ��ī��Ʈ�� ���ҽ�Ű�� ��� �����ϸ� �� Ÿ���� Ŭ����ó�� �մϴ�.
    /// ��� Ÿ���� Ŭ���� ó���Ǹ� TargetCountManager���� ���������� �¸� ó���մϴ�.
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
