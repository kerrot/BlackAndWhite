using UnityEngine;
using System.Collections;

public class PlayerTime : SingletonMonoBehaviour<PlayerTime> {

    Animator anim;

    float tmpTimeScale;

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

        anim.speed = playerSpeed / speed;
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

    public void TestSkill() 
    {
        SlowMotion(0.2f, 1f);

    }
}
