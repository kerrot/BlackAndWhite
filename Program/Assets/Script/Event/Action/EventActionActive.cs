using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionActive : EventAction
{
    [SerializeField]
    private GameObject obj;
    [SerializeField]
    private bool active;

    public override void Launch()
    {
        if (obj)
        {
            obj.SetActive(active);
        }
    }
}
