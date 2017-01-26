using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionAddEvent : EventAction
{
    [SerializeField]
    private GameEvent ev;

    GameEventSystem sys;

    private void Start()
    {
        sys = GameObject.FindObjectOfType<GameEventSystem>();
        if (ev)
        {
            ev.gameObject.OnDestroyAsObservable().Subscribe(_ => DestroyObject(gameObject)).AddTo(this);
        }
    }

    public override void Launch()
    {
        if (ev && sys)
        {
            sys.AddEvent(ev);
        }
    }
}
