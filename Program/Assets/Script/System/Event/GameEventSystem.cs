using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEventSystem : MonoBehaviour {

    [SerializeField]
    private List<GameEvent> events = new List<GameEvent>();

    void Start()
    {
        events.ForEach(e => e.Launch());
    }

    public void AddEvent(GameEvent e)
    {
        if (e && !events.Contains(e))
        {
            events.Add(e);
            e.Launch();
        }
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
