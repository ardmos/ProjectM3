using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        Ready,
        Play,
        Win,
        Lose
    }

    public static GameManager Instance;

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
                OnLose.Invoke(); // Lose시 동작 아직 미구현
                break;
            default:
                Debug.LogError($"유효한 게임 스테이트가 아닙니다.");
                break;
        }
    }

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
