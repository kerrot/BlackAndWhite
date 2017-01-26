using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionExplosionAttacked : EventCondition
{
    private void Start()
    {
        EnemyManager.OnExplosionAttacked.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
