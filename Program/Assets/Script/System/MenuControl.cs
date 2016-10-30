using UnityEngine;
using System.Collections;

public class MenuControl : MonoBehaviour {
    [SerializeField]
    private GameObject next;


	public void StageClear()
    {
        Pause();
        next.SetActive(true);
    }

    public void Pause()
    {
        next.SetActive(false);
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GamePause();
        }
    }

    public void BacktoGame()
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GameResume();
        }

        gameObject.SetActive(false);
    }
}
