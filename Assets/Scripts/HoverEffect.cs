using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
