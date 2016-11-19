using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueSkill : MonoBehaviour {
    [SerializeField]
    private GameObject skillObjeect;
    [SerializeField]
    private float period;

    float startTime;
    float range;
    List<Vector3> points = new List<Vector3>();

    void Start ()
    {
        range = skillObjeect.GetComponent<SphereCollider>().radius * 2;
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnEnableAsObservable().Subscribe(_ => UniRxOnEnable());
    }
	
	void UniRxUpdate()
    {
	    if (Time.time - startTime > period)
        {
            gameObject.SetActive(false);
            return;
        }

        if (points.TrueForAll(p => Vector3.Distance(transform.position, p) > range))
        {
            points.Add(transform.position);

            Instantiate(skillObjeect, transform.position, Quaternion.Euler(-90, 0, 0));
        }
	}

    void UniRxOnEnable()
    {
        startTime = Time.time;
        points.Clear();
    }
}
