using UnityEngine;
using System.Collections;

public abstract class EventCondition : MonoBehaviour
{
    [SerializeField]
    private int count = 1;

    public abstract void Register();

    public bool Complete { get { return complete; } }

    protected bool complete;
}
