using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerTime : MonoBehaviour {

    Animator anim;

    Dictionary<MonoBehaviour, float> speeds = new Dictionary<MonoBehaviour, float>();

    float baseSpeed = 1f;

    void Awake() 
    {
        anim = GetComponent<Animator>();

		SlowMotion (1f, 1f);
    }

	public void SlowMotion(float speed, float playerSpeed) 
    {
        if (speed == 0) 
        {
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
