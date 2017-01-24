using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEvent : MonoBehaviour {

    [SerializeField]
    private List<EventCondition> conditions = new List<EventCondition>();
    [SerializeField]
    private List<EventAction> actions = new List<EventAction>();
    [SerializeField]
    private int repeat = 0;


    public void Launch()
    {
        ConditionInit();
    }

    void ConditionInit()
    {
        conditions.ForEach(c =>
        {
            c.Register();
            //c.OnComplete = ConditionComplete;
        });
    }

    void ConditionComplete(EventCondition cond)
    {
        if (conditions.TrueForAll(c => c.Complete))
        {
            actions.ForEach(a => a.Launch());

            if (repeat > 0)
            {
                ConditionInit();
            }
            else
            {
                //OnComplete
            }
        }
    }
}
