using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionCoreUnion : EventCondition
{
    private void Start()
    {
        CorePeace.OnUnion.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
