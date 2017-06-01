using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionOpeningEnd : EventCondition
{
    private void Start()
    {
        OpeningRTM rtm = GameObject.FindObjectOfType<OpeningRTM>();
        if (rtm)
        {
            rtm.OnOpeningEnd.Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
        }
    }
}
