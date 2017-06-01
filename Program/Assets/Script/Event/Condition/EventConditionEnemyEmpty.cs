using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionEnemyEmpty : EventCondition
{
    private void Start()
    {
        EnemyManager.OnEnemyEmpty.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
