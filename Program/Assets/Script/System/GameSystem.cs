using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSystem : SingletonMonoBehaviour<GameSystem>
{
    [SerializeField]
    NumberDisplayUI combo;
    [SerializeField]
    NumberDisplayUI multiSlash;

    private Subject<int> comboSubject = new Subject<int>();
    private Subject<int> multiSlashSubject = new Subject<int>();
    public IObservable<int> OnCombo { get { return comboSubject; } }
    public IObservable<int> OnMultiSlash { get { return multiSlashSubject; } }

    int slashCount = 0;

    bool pause = false;
    float tmpTimeScale = 0;

    GameState state = GameState.GAME_STATE_PLAYING;
    public enum GameState
    {
        GAME_STATE_PLAYING,
        GAME_STATE_PAUSE,
        GAME_STATE_RTM,
        GAME_STATE_GAMEOVER,
    }
    public GameState State { get { return state; } }

    public void Attack()
    {
        slashCount = 0;
    }

    public void ComboSlash()
    {
        ++slashCount;
        if (combo)
        {
            combo.Display(slashCount);
        }

        comboSubject.OnNext(slashCount);
    }

    public void KillInOneTime(int num)
    {
        if (multiSlash && num > 1)
        {
            multiSlash.Display(num);
        }

        multiSlashSubject.OnNext(num);
    }

    public void GamePause()
    {
        if (pause)
        {
            return;
        }

        tmpTimeScale = Time.timeScale;
        Time.timeScale = 0;
        
        state = GameState.GAME_STATE_PAUSE;
        pause = true;
    }

    public void GameResume()
    {
        pause = false;

        Time.timeScale = tmpTimeScale;
        state = GameState.GAME_STATE_PLAYING;
    }

    public void GameOver()
    {
        state = GameState.GAME_STATE_GAMEOVER;
        Observable.FromCoroutine(RestartGame).Subscribe();
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3f);

        GameScene.ReStartGame();
    }

    public void RTM()
    {
        state = GameState.GAME_STATE_RTM;
    }
}
