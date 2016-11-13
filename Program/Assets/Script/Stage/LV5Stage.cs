using UnityEngine;
using System.Collections;

public class LV5Stage : MonoBehaviour {
    [SerializeField]
    private GameObject readme;
    [SerializeField]
    private MenuControl menu;

    // Use this for initialization
    void Start () {
        readme.SetActive(false);

        OpeningRTM opening = GameObject.FindObjectOfType<OpeningRTM>();
        if (opening)
        {
            opening.OnOpeningEnd += ReadMe;
        }

        EnemyGenerator enemies = GameObject.FindObjectOfType<EnemyGenerator>();
        if (enemies)
        {
            enemies.OnEnemyEmpty += StageClear;
        }
	}

    public void Resume()
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GameResume();
        }

        readme.SetActive(false);
    }

    void ReadMe()
    {
        ShowReadMe(readme);
    }

    void ShowReadMe(GameObject obj)
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GamePause();
        }

        obj.SetActive(true);
    }

    void StageClear()
    {
        StartCoroutine(ShowNextStage());
    }

    IEnumerator ShowNextStage()
    {
        yield return new WaitForSeconds(2f);
        
        menu.gameObject.SetActive(true);
        menu.StageClear();
    }
}
