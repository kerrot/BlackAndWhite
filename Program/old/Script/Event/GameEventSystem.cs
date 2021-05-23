using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// management of game events, create, destroy...
public class GameEventSystem : MonoBehaviour {

    [SerializeField]
    private List<GameEvent> events = new List<GameEvent>();

    void Start()
    {
        events.ForEach(e => Register(e));
    }

    public void AddEvent(GameEvent e)
    {
        if (e && !events.Contains(e))
        {
            events.Add(e);
            Register(e);
        }
    }

    void Register(GameEvent e)
    {
        e.Launch();
        e.OnComplete.Subscribe(ev => DestroyEvent(ev)).AddTo(this);
    }

    void DestroyEvent(GameEvent e)
    {
        if (e && events.Contains(e))
        {
            events.Remove(e);
            DestroyObject(e);
        }
    }
}
