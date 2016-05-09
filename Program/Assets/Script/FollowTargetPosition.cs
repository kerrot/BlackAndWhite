using UnityEngine;
using System.Collections;

public class FollowTargetPosition : MonoBehaviour {

	public Transform target;          
    public bool useSmoothing = true;
	public float smoothing = 5f;       

	Vector3 offset;                     

	void Start ()
	{
		offset = transform.position - target.position;
	}

	void FixedUpdate ()
	{
		Vector3 targetCamPos = target.position + offset;

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
