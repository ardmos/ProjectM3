using System;
using UnityEngine;

/// <summary>
/// 게임 흐름을 관리하는 클래스입니다
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum State
    {
        Ready,
        Play,
        Win,
        Lose
    }

    public MoveCountManager MoveCountManager;
    public ScoreManager ScoreManager;

    public event Action OnReady;
    public event Action OnPlay;
    public event Action OnWin;
    public event Action OnLose;

    [SerializeField] private State state = State.Ready;

    private void Awake()
    {
        Instance = this;
        UpdateState(State.Play);
    }

    public void UpdateState(State newState)
    {
        state = newState;

        RunStateMachine();
    }

    public State GetGameState() => state;

    /// <summary>
    /// State를 통해 게임의 흐름을 관리해주는 메서드입니다
    /// </summary>
    private void RunStateMachine()
    {
        switch (state)
        {
            case State.Ready:
                break;
            case State.Play:
                break;
            case State.Win:
                // 효과음 재생
                SoundManager.Instance.PlaySFX(SoundManager.SFX.Win);
                // 남은 무브 수를 점수로 환산시킵니다. 
                ChageLeftMovesToScore();
                // 게임 결과 세이브
                PlayerDataManager.Instance.UpdatePlayerStageClearData(LevelData.Instance.Level,ScoreManager.StarScore);
                OnWin.Invoke();
                break;
            case State.Lose:
                // 효과음 재생
                SoundManager.Instance.PlaySFX(SoundManager.SFX.Lose);
                OnLose.Invoke();
                break;
            default:
                Debug.LogError($"유효한 게임 스테이트가 아닙니다.");
                break;
        }
    }

    /// <summary>
    /// 게임 클리어시 남은 무브 숫자들을 점수로 치환해주는 메서드입니다.
    /// </summary>
    private void ChageLeftMovesToScore()
    {
        int moveCount = MoveCountManager.MoveCount;

        if( moveCount > 0 )
        {
            ScoreManager.AddScore( moveCount * 40 );
            MoveCountManager.UpdateMoveCountTextUI(LevelData.Instance.MoveMaxCount);
        }
    }
}
