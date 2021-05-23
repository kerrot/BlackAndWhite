using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionEnemyBreak : EventCondition
{
    private void Start()
    {
        EnemyManager.OnEnemyCanSlash.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
