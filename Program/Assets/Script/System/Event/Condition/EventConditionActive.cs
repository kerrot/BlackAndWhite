using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionActive : EventCondition
{
    [SerializeField]
    private GameObject obj;
    [SerializeField]
    private bool active;

    private void Start()
    {
        if (obj)
        {
            obj.UpdateAsObservable().Where(_ => obj.activeSelf == active).Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
        }
    }
}
