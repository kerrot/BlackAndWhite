using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionImmunity : EventCondition
{
    [SerializeField]
    private ImmunityAura aura;

    private void Start()
    {
        if (aura)
        {
            aura.OnBlock.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
            aura.OnDestroyAsObservable().Subscribe(_ => DestroyObject(gameObject)).AddTo(this);
        }
    }
}
