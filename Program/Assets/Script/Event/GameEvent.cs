using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

// game event: in certain codion do certain action
public class GameEvent : MonoBehaviour {

    [SerializeField]
    private List<EventCondition> conditions = new List<EventCondition>();
    [SerializeField]
    private List<ActionSetting> preAct = new List<ActionSetting>();     //actions when event launched
    [SerializeField]
    private List<ActionSetting> postAct = new List<ActionSetting>();    //actions when condition fullfilled
    [SerializeField]
    private bool infinity;                                              // always restart

    private Subject<GameEvent> complete = new Subject<GameEvent>();
    public UniRx.IObservable<GameEvent> OnComplete { get { return complete; } }

    Dictionary<EventCondition, bool> state = new Dictionary<EventCondition, bool>();

    [Serializable]
    public struct ActionSetting
    {
        public EventAction act;
        public float delay;
    }


    public void Launch()
    {
        preAct.ForEach(a => Action(a));
        ConditionInit();
    }

    // init condition state and register event
    void ConditionInit()
    {
        conditions.ForEach(c =>
        {
            state.Add(c, false);

            // if any conditoin completed
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = c.OnComplete.Subscribe(cond =>
            {
                ConditionComplete(cond);
                disposable.Dispose();
            }).AddTo(this);
        });
    }

    // Check all conditions and do the action
    void ConditionComplete(EventCondition cond)
    {
        if (state.ContainsKey(cond))
        {
            state[cond] = true;
            if (state.All(s => s.Value))
            {
                postAct.ForEach(a => Action(a));

                if (infinity)
                {
                    state.Clear();
                    ConditionInit();
                }
                else
                {
                    complete.OnNext(this);
                }
            }
        }
    }

    // do the action
    void Action(ActionSetting a)
    {
        if (a.delay > 0)
        {
            Observable.Timer(TimeSpan.FromSeconds(a.delay)).Subscribe(_ => a.act.Launch());
        }
        else
        {
            a.act.Launch();
        }
    }
}
