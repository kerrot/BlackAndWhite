using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

// camera shake effect
public class ShakeCamera : MonoBehaviour
{
    [SerializeField]
    private float time;             // last time
    [SerializeField]
    private float offset;           // shake power
    [SerializeField]
    private float frequency;        // shake frequency

    FollowTargetPosition follower;  // base position

    float counter = 0;
    float startTime = 0;

    System.IDisposable subject;

    void Start()
    {
        startTime = Time.time;
        follower = GetComponent<FollowTargetPosition>();
    }

    void OnEnable()
    {
        startTime = Time.time;
        // after all update
        subject = this.LateUpdateAsObservable().Subscribe(_ => UniRxLateUpdate());
    }

	void UniRxLateUpdate()
    {
        if (!follower)
        {
            return;
        }

        if (offset > 0 && frequency > 0)
        {
            counter += Time.deltaTime;
            if (counter > frequency)
            {
                counter -= frequency;

                // random position offset
                Vector2 randomVector = Random.insideUnitCircle * offset;

                Vector3 shakeOffset = transform.up * randomVector.y + transform.right * randomVector.x;

                transform.position = follower.CurrentPosition + shakeOffset;
            }
        }

        if (time > 0 && Time.time - startTime > time)
        {
            enabled = false;
            subject.Dispose();
            transform.position = follower.CurrentPosition;
        }
    }
}