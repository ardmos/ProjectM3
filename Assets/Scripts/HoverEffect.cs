using DG.Tweening;
using UnityEngine;

/// <summary>
/// 게임 오브젝트가 호버링 할 수 있도록 만들어주는 컴포넌트입니다.
/// 타이틀씬의 타이틀 이미지가 사용중입니다.
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

        // 호버링 효과 시작
        StartHoverEffect();
    }

    /// <summary>
    /// 호버링 이펙트 시작
    /// </summary>
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
