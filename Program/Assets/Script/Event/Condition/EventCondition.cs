using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;
using System.Collections;

// only trigger event
public abstract class EventCondition : MonoBehaviour
{
    protected Subject<EventCondition> completeSubject = new Subject<EventCondition>();
    public IObservable<EventCondition> OnComplete { get { return completeSubject; } }
}
