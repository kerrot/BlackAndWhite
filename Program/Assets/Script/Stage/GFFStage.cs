using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class GFFStage : MonoBehaviour {
    [SerializeField]
    protected GameObject core;
    [SerializeField]
    protected GameObject ending;


    void Awake ()
    {
	    if (core)
        {
            core.OnDestroyAsObservable().Subscribe(_ => GameClear()).AddTo(this);
        }
	}

    void GameClear()
    {
        if (ending)
        {
            ending.SetActive(true);
        }

        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            Time.timeScale = 0.3f;
            system.RTM();
        }
    }
}
