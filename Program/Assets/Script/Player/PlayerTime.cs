﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// for slow motion
public class PlayerTime : MonoBehaviour {

    Animator anim;

    Dictionary<MonoBehaviour, float> speeds = new Dictionary<MonoBehaviour, float>();

    float baseSpeed = 1f;
    GameSystem system;

    void Awake() 
    {
        anim = GetComponent<Animator>();
        system = GameObject.FindObjectOfType<GameSystem>();

        SlowMotion (1f, 1f);
    }

	public void SlowMotion(float speed, float playerSpeed) 
    {
        if (speed == 0 || system.State == GameSystem.GameState.GAME_STATE_PAUSE) 
        {
            Debug.Log("Slow Motion Error");
            return;
        }

        Time.timeScale = speed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        baseSpeed = playerSpeed / speed;

        UpdateSpeed();
    }

    public void CancelSpeed(MonoBehaviour behaviour)
    {
        if (speeds.ContainsKey(behaviour))
        {
            speeds.Remove(behaviour);

            UpdateSpeed();
        }
    }

    // for specially speed up, managed by the one who do the speed up
    public void SpeedChange(float speed, MonoBehaviour behaviour)
    {
        if (behaviour)
        {
            if (speeds.ContainsKey(behaviour))
            {
                speeds[behaviour] = speed;
            }
            else
            {
                speeds.Add(behaviour, speed);
                behaviour.OnDestroyAsObservable().Subscribe(_ => CancelSpeed(behaviour)).AddTo(this);
            }

            UpdateSpeed();
        }
    }

    void UpdateSpeed()
    {
        anim.speed = baseSpeed + speeds.Sum(s => s.Value);
    }
}
