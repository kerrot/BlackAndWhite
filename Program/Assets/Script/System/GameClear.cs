using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameClear : MonoBehaviour
{

    [SerializeField]
    Text scoreUI;
    [SerializeField]
    Button restart;

    int now = 0;
    int score;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());

        score = GameSystem.GetScore();
    }

    void UniRxUpdate()
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