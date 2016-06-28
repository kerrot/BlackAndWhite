using UnityEngine;
using System.Collections;

public class PlayerTime : SingletonMonoBehaviour<PlayerTime> {

    Animator anim;

    float tmpTimeScale = 1;
    float timeFactor = 1;

    void Awake() 
    {
        anim = GetComponent<Animator>();
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

    public void PauseGame()
    {
        tmpTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void ResumeGame() 
    {
        Time.timeScale = tmpTimeScale;
    }

    public float GetPlayerTimeFactor()
    {
        return timeFactor;
    }
}
