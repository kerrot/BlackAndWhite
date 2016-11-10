﻿using UniRx;
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
    }

	void Start ()
	{
		offset = transform.position - follow.transform.position;
        //this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

	void LateUpdate()
	{
		Vector3 currentPosition = follow.transform.position + offset;

        if (useSmoothing)
        {
            currentPosition = Vector3.Lerp(transform.position, currentPosition, smoothing * Time.deltaTime);
        }
		
        transform.position = currentPosition;
	}
}
