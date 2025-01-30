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

        // 호버링 효과 시작
        StartHoverEffect();
    }

    void StartHoverEffect()
    {
        // 위로 이동
        rectTransform.DOAnchorPos3D(startPosition + Vector3.up * hoverAmount, hoverDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // 아래로 이동
                rectTransform.DOAnchorPos3D(startPosition, hoverDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(StartHoverEffect);
            });
    }

    void OnDisable()
    {
        // 오브젝트가 비활성화될 때 Tween 정지
        rectTransform.DOKill();
    }
}
