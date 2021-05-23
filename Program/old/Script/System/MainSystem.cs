using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;
using UniRx.Triggers;

public class MainSystem : MonoBehaviour
{
    private Subject<Unit> UpdateSubject = new Subject<Unit>();
    public IObservable<Unit> OnUpdate { get { return UpdateSubject; } }
    private Subject<Unit> FixedUpdateSubject = new Subject<Unit>();
    public IObservable<Unit> OnFixedUpdate { get { return FixedUpdateSubject; } }
    private Subject<Unit> LateUpdateSubject = new Subject<Unit>();
    public IObservable<Unit> OnLateUpdate { get { return LateUpdateSubject; } }

    private static MainSystem instance;
    public static MainSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("System");
                instance = obj.AddComponent<MainSystem>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;

        this.UpdateAsObservable().Subscribe(_ => UpdateSubject.OnNext(Unit.Default)).AddTo(this);
        this.FixedUpdateAsObservable().Subscribe(_ => FixedUpdateSubject.OnNext(Unit.Default)).AddTo(this);
        this.LateUpdateAsObservable().Subscribe(_ => LateUpdateSubject.OnNext(Unit.Default)).AddTo(this);
    }
}
