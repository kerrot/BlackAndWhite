using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionBlueSpecial : EventCondition
{
    private void Start()
    {
        BlueEnemySpecial.OnBlock.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
    }
}
