using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventConditionKeyDown : EventCondition
{
    [SerializeField]
    private string keyText;

    private void Start()
    {
        this.UpdateAsObservable().Where(_ => Input.GetButtonDown(keyText)).Subscribe(_ => completeSubject.OnNext(this));
    }
}
