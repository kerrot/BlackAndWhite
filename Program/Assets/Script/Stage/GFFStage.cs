using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class GFFStage : MonoBehaviour {
    [SerializeField]
    protected GameObject core;
    [SerializeField]
    protected GameObject ending;
    [SerializeField]
    protected CorePeace red;
    [SerializeField]
    protected CorePeace green;
    [SerializeField]
    protected CorePeace blue;
    [SerializeField]
    protected KnightBattle redBoss;
    [SerializeField]
    protected KnightBattle greenBoss;
    [SerializeField]
    protected KnightBattle blueBoss;

    void Awake()
    {
        if (core)
        {
            core.OnDestroyAsObservable().Subscribe(_ => GameClear()).AddTo(this);
        }

        var unionSubject = new SingleAssignmentDisposable();
        unionSubject.Disposable = CorePeace.OnUnion.Subscribe(_ =>
        {
            redBoss.gameObject.GetComponent<EnemyHP>().CanRecover = true;
            greenBoss.gameObject.GetComponent<EnemyHP>().CanRecover = true;
            blueBoss.gameObject.GetComponent<EnemyHP>().CanRecover = true;

            red.Register();
            green.Register();
            blue.Register();

            unionSubject.Dispose();
        }).AddTo(this);
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
