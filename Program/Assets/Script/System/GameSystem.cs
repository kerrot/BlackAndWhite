using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSystem : SingletonMonoBehaviour<GameSystem>
{
    [SerializeField]
    int multiBonus;
    [SerializeField]
    int comboBonus;
    [SerializeField]
    int singleKillScore;
    [SerializeField]
    Text scoreUI;
    [SerializeField]
    Text timeUI;
    [SerializeField]
    int limit;
    [SerializeField]
    NumberDisplayUI combo;
    [SerializeField]
    NumberDisplayUI multiSlash;
	[SerializeField]
	GameObject inGame;
	[SerializeField]
	GameObject endGame;

    float startTime;
    int slashCount = 0;
    static int score = 0;

	static public int GetScore()
	{
		return score;
	}

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        startTime = Time.realtimeSinceStartup;
		score = 0;
    }

    void UniRxUpdate()
    {
        int now = limit - (int)(Time.realtimeSinceStartup - startTime);

        if (now < 0)
        {
			TimeUp ();
        }

        timeUI.text = now.ToString();
    }

	void TimeUp()
	{
		inGame.SetActive (false);
		endGame.SetActive (true);
	}

    public void Attack()
    {
        slashCount = 0;
    }

    public void ComboSlash()
    {
        ++slashCount;
        combo.Display(slashCount);
        score += comboBonus * slashCount;
        UpdateScore();
    }

    public void KillInOneTime(int num)
    {
        if (num > 1)
        {
            multiSlash.Display(num);
            score += multiBonus * num;
        }

        score += singleKillScore * num;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreUI.text = score.ToString();
    }
}
