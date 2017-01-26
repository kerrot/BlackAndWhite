using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionGreenSpecial : EventCondition
{
    private void Start()
    {
        GreenEnemySpecial.OnBlock.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
