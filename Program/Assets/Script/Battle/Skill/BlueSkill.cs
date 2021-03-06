﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// spawn water object once every [range] distance
public class BlueSkill : Skill {
    [SerializeField]
    private GameObject water;
    [SerializeField]
    private float range;

    List<Vector3> points = new List<Vector3>();

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        if (points.TrueForAll(p => Vector3.Distance(transform.position, p) > range))
        {
            points.Add(transform.position);

            GameObject obj = Instantiate(water, transform.position, Quaternion.Euler(-90, 0, 0));
            obj.transform.parent = GameObject.FindObjectOfType<EnemyManager>().transform;
        }
	}

    void OnEnable()
    {
        points.Clear();
    }
}
