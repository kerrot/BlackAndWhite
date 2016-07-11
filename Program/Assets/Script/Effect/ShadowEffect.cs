using UniRx;
using UniRx.Triggers;
using System.Linq;
using UnityEngine;
using System.Collections;

public class ShadowEffect : MonoBehaviour {

    [SerializeField]
    private float time;

    private float start;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        start = Time.realtimeSinceStartup;
    }

    void UniRxUpdate()
    {
        

        if (Time.realtimeSinceStartup - start > time)
        {
            Destroy(gameObject);
        }
    }
}
