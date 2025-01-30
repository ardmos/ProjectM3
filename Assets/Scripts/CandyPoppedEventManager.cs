using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OnPopped 이벤트의 래퍼 클래스입니다. 
/// 이벤트 핸들러들의 실행 순서를 관리합니다.
/// </summary>
public class CandyPoppedEventManager : MonoBehaviour
{
    public ScoreManager ScoreManager;
    public TargetCountManager TargetCountManager;

    private void Start()
    {
        GameBoardManager.Instance.OnPopped += HandlePoppedEvent;
    }

    private void HandlePoppedEvent(List<Candy> poppedCandies)
    {
        // 먼저 ScoreManager의 AddScore 실행
        ScoreManager.AddScoreByCandy(poppedCandies);

        // 그 다음 TargetCountManager의 CheckTargetClear 실행
        TargetCountManager.CheckTargetClear(poppedCandies);
    }
}
