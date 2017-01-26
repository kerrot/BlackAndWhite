using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionPlayerDanger : EventCondition
{
    [SerializeField]
    private bool danger;

    private void Start()
    {
        PlayerHurt hurt = GameObject.FindObjectOfType<PlayerHurt>();
        if (hurt)
        {
            hurt.OnDanger.Subscribe(v =>
            {
                if (danger == v)
                {
                    completeSubject.OnNext(this);
                }
            }).AddTo(this);
        }
    }
}
