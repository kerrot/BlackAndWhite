using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameEvent : MonoBehaviour {

    [SerializeField]
    private List<EventCondition> conditions = new List<EventCondition>();
    [SerializeField]
    private List<ActionSetting> preAct = new List<ActionSetting>();
    [SerializeField]
    private List<ActionSetting> postAct = new List<ActionSetting>();
    [SerializeField]
    private bool infinity;

    private Subject<GameEvent> complete = new Subject<GameEvent>();
    public IObservable<GameEvent> OnComplete { get { return complete; } }

    Dictionary<EventCondition, bool> state = new Dictionary<EventCondition, bool>();

    //[Serializable]
    //public struct ConditionSetting
    //{
    //    public AttackType type;
    //    public ElementType element;
    //}

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

    void ConditionInit()
    {
        conditions.ForEach(c =>
        {
            state.Add(c, false);

            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = c.OnComplete.Subscribe(cond =>
            {
                ConditionComplete(cond);
                disposable.Dispose();
            }).AddTo(this);
        });
    }

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
