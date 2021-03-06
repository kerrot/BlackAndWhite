﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

// trigger boss core event
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
    protected GameObject sub;
    [SerializeField]
    private FollowTargetPosition follow;
    [SerializeField]
    private AudioClip coreSE;
    [SerializeField]
    private GameObject coreHint;

    GameSystem system;

    System.IDisposable redDis;
    System.IDisposable greenDis;
    System.IDisposable blueDis;
    System.IDisposable RTmDis;
    System.IDisposable coreDis;
    System.IDisposable coreEnableDis;

    void Awake()
    {
        this.OnDestroyAsObservable().Subscribe(_ => 
        {
            Time.timeScale = 1f;
        });

        // Game clear
        if (core)
        {
            coreEnableDis = core.OnEnableAsObservable().Subscribe(_ => AudioHelper.PlaySE(core, coreSE));

            core.OnDestroyAsObservable().Subscribe(_ => 
            {
                coreDis.Dispose();
                coreEnableDis.Dispose();
                GameClear();
            }).AddTo(this);
        }


        // core union after boss first revival
        var unionSubject = new SingleAssignmentDisposable();
        unionSubject.Disposable = CorePeace.OnUnion.Subscribe(_ =>
        {
            redBoss.gameObject.GetComponent<EnemyHP>().CanRecover = true;
            greenBoss.gameObject.GetComponent<EnemyHP>().CanRecover = true;
            blueBoss.gameObject.GetComponent<EnemyHP>().CanRecover = true;

            red.Register();
            green.Register();
            blue.Register();

            sub.SetActive(false);

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

            // when all three boss dead
            if (redDis != null && blueDis != null && greenDis != null && core.activeSelf)
            {
                redDis.Dispose();
                blueDis.Dispose();
                greenDis.Dispose();
                redDis = null;
                blueDis = null;
                greenDis = null;
                coreHint.SetActive(true);

                Observable.TimerFrame(200).Subscribe(o =>
                {
                    system.GameResume();
                    follow.follow = GameObject.FindObjectOfType <PlayerMove>().gameObject;
                    follow.useSmoothing = false;
                    coreHint.SetActive(false);
                });
            }
        });
    }

    void UnionRTM()
    {
        // the camera work of  core show up
        RTmDis = CorePeace.OnUnion.Take(1).Subscribe(v =>
        {
            RTmDis.Dispose();

            system.GamePause();
            core.transform.position = v;

            follow.follow = core;
            follow.useSmoothing = true;

            Observable.TimerFrame(150).Subscribe(_ =>
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
