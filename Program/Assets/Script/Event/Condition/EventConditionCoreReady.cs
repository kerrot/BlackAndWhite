using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionCoreReady : EventCondition
{
    [SerializeField]
    private CorePeace core;

    private void Start()
    {
        if (core)
        {
            core.OnReady.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
        }
    }
}
