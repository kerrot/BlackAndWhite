using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class FollowTargetPosition : MonoBehaviour
{
    [SerializeField]
    public GameObject follow;
    [SerializeField]
    public bool useSmoothing = true;
    [SerializeField]
    private float smoothing = 1f;       

	Vector3 offset;

    Vector3 currentPosition;
    public Vector3 CurrentPosition { get { return currentPosition; } }

    void Awake()
    {
        currentPosition = transform.position;
        offset = transform.position - follow.transform.position;
    }

	void Start ()
	{
		this.LateUpdateAsObservable().Subscribe (_ => UniRxLateUpdate ());
    }

	void UniRxLateUpdate()
	{
        if (follow)
        {
            currentPosition = follow.transform.position + offset;

            if (useSmoothing)
            {
                currentPosition = Vector3.Lerp(transform.position, currentPosition, smoothing * Time.unscaledDeltaTime);
            }

            transform.position = currentPosition;
        }
	}
}
