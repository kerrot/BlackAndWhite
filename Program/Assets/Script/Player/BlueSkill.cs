using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueSkill : MonoBehaviour {
    [SerializeField]
    private GameObject skillObjeect;
    [SerializeField]
    private float range;
    [SerializeField]
    private float period;

    float startTime;

    List<Vector3> points = new List<Vector3>();

    void Start ()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
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

            Instantiate(skillObjeect, transform.position, Quaternion.identity);
        }
	}

    void OnEnable()
    {
        startTime = Time.time;
        points.Clear();
    }
}
