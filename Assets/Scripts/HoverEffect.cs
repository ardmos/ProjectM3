using DG.Tweening;
using UnityEngine;

/// <summary>
/// ���� ������Ʈ�� ȣ���� �� �� �ֵ��� ������ִ� ������Ʈ�Դϴ�.
/// Ÿ��Ʋ���� Ÿ��Ʋ �̹����� ������Դϴ�.
/// </summary>
public class HoverEffect : MonoBehaviour
{
    public float hoverAmount = 10f;
    public float hoverDuration = 1f;

    private RectTransform rectTransform;
    private Vector3 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition3D;

        // ȣ���� ȿ�� ����
        StartHoverEffect();
    }

    /// <summary>
    /// ȣ���� ����Ʈ ����
    /// </summary>
    void StartHoverEffect()
    {
        // ���� �̵�
        rectTransform.DOAnchorPos3D(startPosition + Vector3.up * hoverAmount, hoverDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // �Ʒ��� �̵�
                rectTransform.DOAnchorPos3D(startPosition, hoverDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(StartHoverEffect);
            });
    }

    void OnDisable()
    {
        // ������Ʈ�� ��Ȱ��ȭ�� �� Tween ����
        rectTransform.DOKill();
    }
}
