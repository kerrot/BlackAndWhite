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
    [SerializeField]
    private FollowTargetPosition follow;

    GameSystem system;

    System.IDisposable redDis;
    System.IDisposable greenDis;
    System.IDisposable blueDis;
    System.IDisposable RTmDis;
    System.IDisposable coreDis;

    void Awake()
    {
        if (core)
        {
            core.OnDestroyAsObservable().Subscribe(_ => 
            {
                coreDis.Dispose();
                GameClear();
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

            UnionRTM();

        }).AddTo(this);

        system = GameObject.FindObjectOfType<GameSystem>();
    }

    private void Start()
    {
        coreDis = this.UpdateAsObservable().Subscribe(_ => 
        {
            if (core)
            {
                core.SetActive(red.UnionReach && green.UnionReach && blue.UnionReach);
            }


            if (redDis != null && blueDis != null && greenDis != null && core.activeSelf)
            {
                redDis.Dispose();
                blueDis.Dispose();
                greenDis.Dispose();


                Observable.TimerFrame(200).Subscribe(o =>
                {
                    system.GameResume();
                    follow.follow = GameObject.FindObjectOfType <PlayerMove>().gameObject;
                    follow.useSmoothing = false;
                });
            }
        });
    }

    void UnionRTM()
    {
        RTmDis = CorePeace.OnUnion.Take(1).Subscribe(v =>
        {
            RTmDis.Dispose();

            system.GamePause();
            core.transform.position = v;

            follow.follow = core;
            follow.useSmoothing = true;

            Observable.TimerFrame(250).Subscribe(_ =>
            {
                redDis = red.gameObject.UpdateAsObservable()
                          .TakeWhile(r => red.gameObject.activeSelf)
                          .Subscribe(c => red.transform.position += (v - red.transform.position) * Time.unscaledDeltaTime).AddTo(this);
                greenDis = green.gameObject.UpdateAsObservable()
                              .TakeWhile(r => green.gameObject.activeSelf)
                              .Subscribe(c => green.transform.position += (v - green.transform.position) * Time.unscaledDeltaTime).AddTo(this);
                blueDis = blue.gameObject.UpdateAsObservable()
                              .TakeWhile(r => blue.gameObject.activeSelf)
                              .Subscribe(c => blue.transform.position += (v - blue.transform.position) * Time.unscaledDeltaTime).AddTo(this);
            });
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
            PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
            if (player)
            {
                player.enabled = false;
            }
        }
    }
}
