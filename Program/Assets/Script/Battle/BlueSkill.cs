using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueSkill : Skill {
    [SerializeField]
    private GameObject water;
    float range;
    List<Vector3> points = new List<Vector3>();

    void Start()
    {
        range = water.GetComponent<SphereCollider>().radius * 2;
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        if (points.TrueForAll(p => Vector3.Distance(transform.position, p) > range))
        {
            points.Add(transform.position);

            Instantiate(water, transform.position, Quaternion.Euler(-90, 0, 0));
        }
	}

    void OnEnable()
    {
        points.Clear();
    }
}
