using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionGameState : EventAction
{
    [SerializeField]
    private GameSystem.GameState state;

    GameSystem system;

    private void Start()
    {
        system = GameObject.FindObjectOfType<GameSystem>();
    }

    public override void Launch()
    {
        if (system)
        {
            switch (state)
            {
                case GameSystem.GameState.GAME_STATE_PAUSE:
                    system.GamePause();
                    break;
                case GameSystem.GameState.GAME_STATE_PLAYING:
                    system.GameResume();
                    break;
                case GameSystem.GameState.GAME_STATE_RTM:
                    system.RTM();
                    break;
            }
        }
    }
}
