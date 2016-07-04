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

    float startTime;
    int slashCount = 0;
    int score = 0;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        startTime = Time.realtimeSinceStartup;
    }

    void UniRxUpdate()
    {
        int now = limit - (int)(Time.realtimeSinceStartup - startTime);

        if (now < 0)
        {
            SceneManager.LoadScene("End");
        }

        timeUI.text = now.ToString();
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
