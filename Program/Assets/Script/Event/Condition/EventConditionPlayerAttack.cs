using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionPlayerAttack : EventCondition
{
    private void Start()
    {
        PlayerBattle battle = GameObject.FindObjectOfType<PlayerBattle>();
        if (battle)
        {
            battle.OnAttack.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
        }
    }
}
