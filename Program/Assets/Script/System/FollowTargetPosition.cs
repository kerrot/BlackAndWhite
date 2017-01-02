using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class FollowTargetPosition : MonoBehaviour
{
    [SerializeField]
	private GameObject follow;
    [SerializeField]
    private bool useSmoothing = true;
    [SerializeField]
    private float smoothing = 5f;       

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
            Vector3 currentPosition = follow.transform.position + offset;

            if (useSmoothing)
            {
                currentPosition = Vector3.Lerp(transform.position, currentPosition, smoothing * Time.deltaTime);
            }

            transform.position = currentPosition;
        }
	}
}
