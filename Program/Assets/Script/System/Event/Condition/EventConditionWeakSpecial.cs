using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionWeakSpecial : EventCondition
{
    private void Start()
    {
        WeakSpecial.OnBlock.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
