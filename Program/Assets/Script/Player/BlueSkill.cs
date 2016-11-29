using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueSkill : AuraBattle {
    [SerializeField]
    private GameObject skillObjeect;

    float range;
    List<Vector3> points = new List<Vector3>();

    protected override void AuraStart()
    {
        range = skillObjeect.GetComponent<SphereCollider>().radius * 2;
    }

    protected override void AuraUpdate()
    {
        if (points.TrueForAll(p => Vector3.Distance(transform.position, p) > range))
        {
            points.Add(transform.position);

            Instantiate(skillObjeect, transform.position, Quaternion.Euler(-90, 0, 0));
        }
	}

    protected override void AuraDisappear()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        points.Clear();
        DoRecover();
    }
}
