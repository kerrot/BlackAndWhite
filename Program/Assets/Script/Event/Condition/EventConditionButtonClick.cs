using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventConditionButtonClick : EventCondition
{
    [SerializeField]
    private Button btn;

    private void Start()
    {
        if (btn)
        {
            btn.OnClickAsObservable().Subscribe(_ => completeSubject.OnNext(this)).AddTo(this);
            btn.OnDestroyAsObservable().Subscribe(_ => DestroyObject(gameObject)).AddTo(this);
        }
    }
}
