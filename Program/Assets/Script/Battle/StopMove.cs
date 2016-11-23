using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class StopMove : MonoBehaviour {
    [SerializeField]
    private float lastTime;
	
    public EnemyMove victom { get; set; }

    float startTime;

    void Start()
    {
        startTime = Time.time;
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        if (victom)
        {
            victom.enabled = false;

            if (Time.time - startTime > lastTime)
            {
                victom.enabled = true;
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
