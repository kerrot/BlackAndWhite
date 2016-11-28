using UnityEngine;
using System.Collections;

public class PlayerTime : SingletonMonoBehaviour<PlayerTime> {

    Animator anim;

    float timeFactor = 1;

    void Awake() 
    {
        anim = GetComponent<Animator>();

		SlowMotion (1, 1); 
    }

	public void SlowMotion(float speed, float playerSpeed) 
    {
        if (speed == 0) 
        {
            return;
        }

        Time.timeScale = speed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        timeFactor = playerSpeed / speed;

        anim.speed = timeFactor;
    }
}
