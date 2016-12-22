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
    protected GameObject alert;
    [SerializeField]
    protected CorePeace red;
    [SerializeField]
    protected CorePeace green;
    [SerializeField]
    protected CorePeace blue;
    [SerializeField]
    protected ImmunityAura bossAura;
    [SerializeField]
    protected KnightBattle redBoss;
    [SerializeField]
    protected KnightBattle greenBoss;
    [SerializeField]
    protected KnightBattle blueBoss;
    [SerializeField]
    protected GameObject bossHint;
    [SerializeField]
    protected GameObject sub;

    void Awake()
    {
        if (core)
        {
            core.OnDestroyAsObservable().Subscribe(_ => GameClear()).AddTo(this);
        }

        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            PlayerBattle battle = GameObject.FindObjectOfType<PlayerBattle>();
            if (battle)
            {
                battle.OnDead.Subscribe(_ => system.GameOver()).AddTo(this);
            }
        }

        if (red)
        {
            var bossHintSubject = new SingleAssignmentDisposable();
            if (bossAura)
            {
                bossHintSubject.Disposable = bossAura.OnBlock.Subscribe(_ => 
                {
                    bossHint.SetActive(true);
                    Observable.Timer(System.TimeSpan.FromSeconds(3f)).Subscribe(t => bossHint.SetActive(false));
                }).AddTo(this);
            }

            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = red.OnReady.Subscribe(v =>
            {
                if (v && greenBoss)
                {
                    greenBoss.gameObject.SetActive(true);
                    disposable.Dispose();
                    bossHintSubject.Dispose();
                }
            }).AddTo(this);
        }

        if (green)
        {
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = green.OnReady.Subscribe(v =>
            {
                if (v && blueBoss)
                {
                    blueBoss.gameObject.SetActive(true);
                    disposable.Dispose();
                }
            }).AddTo(this);
        }

        if (blue)
        {
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = green.OnReady.Subscribe(v =>
            {
                if (v)
                {
                    sub.SetActive(false);
                    disposable.Dispose();
                }
            }).AddTo(this);
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

    void BossShow()
    {
        if (alert)
        {
            alert.SetActive(true);
        }
    }
}
