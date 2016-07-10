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

	void Start ()
	{
		offset = transform.position - follow.transform.position;
	}

	void FixedUpdate ()
	{
		Vector3 targetCamPos = follow.transform.position + offset;

        if (useSmoothing)
        {
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
		else
        {
            transform.position = targetCamPos;
        }
	}
}
