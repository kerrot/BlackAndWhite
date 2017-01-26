using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionRedSpecial : EventCondition
{
    private void Start()
    {
        RedEnemySpecial.OnBurn.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
