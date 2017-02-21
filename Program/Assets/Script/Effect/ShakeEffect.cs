using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class ShakeEffect : MonoBehaviour
{
    [SerializeField]
    private float time;
    [SerializeField]
    private float offset;
    [SerializeField]
    private float frequency;

    float counter = 0;

    Vector3 initPos;

    System.IDisposable subject;

    private void Start()
    {
        initPos = transform.localPosition;
    }

    void OnEnable()
    {
        Observable.Timer(System.TimeSpan.FromSeconds(time)).Subscribe(_ => 
        {
            enabled = false;
            subject.Dispose();
            transform.localPosition = initPos;
        }).AddTo(this);

        subject = this.LateUpdateAsObservable().Subscribe(_ => UniRxLateUpdate());
    }

	void UniRxLateUpdate()
    {
        if (offset > 0 && frequency > 0)
        {
            counter += Time.deltaTime;
            if (counter > frequency)
            {
                counter -= frequency;

                Vector3 randomVector = Random.insideUnitSphere * offset;
                transform.localPosition = initPos + randomVector;
            }
        }
    }
}