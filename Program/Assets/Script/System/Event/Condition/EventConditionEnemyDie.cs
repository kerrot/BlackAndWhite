using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionEnemyDie : EventCondition
{
    [SerializeField]
    private EnemyBattle enemy;

    private void Start()
    {
        if (enemy)
        {
            enemy.OnDie.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
            enemy.gameObject.OnDestroyAsObservable().Subscribe(_ => DestroyObject(gameObject)).AddTo(this);
        }
    }
}
