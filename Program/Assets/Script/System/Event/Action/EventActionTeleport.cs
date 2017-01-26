using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionTeleport : EventAction
{
    [SerializeField]
    private GameObject obj;
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Vector3 rotation;

    public override void Launch()
    {
        if (obj)
        {
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
