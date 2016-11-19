using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestStage : MonoBehaviour
{
    [SerializeField]
    Text scoreUI;
    [SerializeField]
    Button restart;
    [SerializeField]
    int multiBonus;
    [SerializeField]
    int comboBonus;
    [SerializeField]
    int singleKillScore;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text timeUI;
    [SerializeField]
    int limit;
    [SerializeField]
    GameObject inGame;
    [SerializeField]
    GameObject endGame;

    int now = 0;

    float startTime;
    static int score = 0;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        startTime = Time.realtimeSinceStartup;

        score = 0;

        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.OnCombo.Subscribe(i => OnCombo(i)).AddTo(this);
            system.OnMultiSlash.Subscribe(i => OnMultiSlash(i)).AddTo(this);
        }
    }

    void UniRxUpdate()
    {
        if (GameSystem.Instance.State == GameSystem.GameState.GAME_STATE_PLAYING)
        {

            int nowTime = limit - (int)(Time.realtimeSinceStartup - startTime);

            if (nowTime < 0)
            {
                TimeUp();
            }

            timeUI.text = nowTime.ToString();
        }
        else if (GameSystem.Instance.State == GameSystem.GameState.GAME_STATE_PAUSE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                now = score;
            }

            scoreUI.text = now.ToString();
            if (now < score)
            {
                ++now;
            }
            else
            {
                restart.gameObject.SetActive(true);
            }
        }
    }

    void TimeUp()
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GamePause();
        }
        inGame.SetActive(false);

        StartCoroutine(GameClear());
    }

    IEnumerator GameClear()
    {
        yield return new WaitForSeconds(0.5f);
        //PlayerSkill.Instance.PowerUsed(100);
        Time.timeScale = 0;
        endGame.SetActive(true);
    }

    void OnCombo(int slashCount)
    {
        score += comboBonus * slashCount;
        UpdateScore();
    }

    void OnMultiSlash(int num)
    {
        if (num > 1)
        {
            score += multiBonus * num;
        }

        score += singleKillScore * num;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }
}