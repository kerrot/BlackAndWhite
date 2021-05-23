using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] GameObject Target;

    Vector3 distance;

    void Awake()
    {
        Init();
    }

    bool init;
    void Init()
    {
        if (init == false && Target != null)
        {
            distance = transform.position - Target.transform.position;
            init = true;
        }
    }

    void LateUpdate()
    {
        if (init)
        {
            transform.position = Target.transform.position + distance;
        }
    }
}
