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

    public delegate void ComboAction(int slashCount);
    static public ComboAction OnCombo;
    public delegate void MultiSlashAction(int num);
    static public MultiSlashAction OnMultiSlash;

    int slashCount = 0;
    
    GameState state = GameState.GAME_STATE_INGAME;
    public enum GameState
    {
        GAME_STATE_INGAME,
        GAME_STATE_OFFGAME,
    }
    public GameState State { get { return state; }
                             set { state = value; }
                            }

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

        if (OnCombo != null)
        {
            OnCombo(slashCount);
        }
    }

    public void KillInOneTime(int num)
    {
        if (multiSlash && num > 1)
        {
            multiSlash.Display(num);
        }

        if (OnMultiSlash != null)
        {
            OnMultiSlash(num);
        }
    }
}
