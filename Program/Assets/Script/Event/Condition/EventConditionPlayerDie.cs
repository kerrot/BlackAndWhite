using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionPlayerDie : EventCondition
{
    private void Start()
    {
        PlayerBattle battle = GameObject.FindObjectOfType<PlayerBattle>();
        if (battle)
        {
            battle.OnDead.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
        }
    }
}
