using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField]
    private float time;
    [SerializeField]
    private float offset;
    [SerializeField]
    private float frequency;

    FollowTargetPosition follower;

    float counter = 0;
    float startTime = 0;

    void Start()
    {
        startTime = Time.time;
        follower = GetComponent<FollowTargetPosition>();
		this.LateUpdateAsObservable().Subscribe (_ => UniRxLateUpdate ());
    }

    void OnEnable()
    {
        startTime = Time.time;
    }

	void UniRxLateUpdate()
    {
        if (offset > 0 && frequency > 0)
        {
            counter += Time.deltaTime;
            if (counter > frequency)
            {
                counter -= frequency;

                Vector2 randomVector = Random.insideUnitCircle * offset;

                Vector3 shakeOffset = transform.up * randomVector.y + transform.right * randomVector.x;

                transform.position = follower.CurrentPosition + shakeOffset;
            }
        }

        if (time > 0 && Time.time - startTime > time)
        {
            enabled = false;
            transform.position = follower.CurrentPosition;
        }
    }
}