using UnityEngine;
using System.Collections;

public class LV2Stage : MonoBehaviour {
    [SerializeField]
    private GameObject readme;
    [SerializeField]
    private MenuControl menu;

    bool showCombo = false;

    // Use this for initialization
    void Start () {
        readme.SetActive(false);

        OpeningRTM opening = GameObject.FindObjectOfType<OpeningRTM>();
        if (opening)
        {
            opening.OnOpeningEnd += Resume;
        }

        EnemyGenerator enemies = GameObject.FindObjectOfType<EnemyGenerator>();
        if (enemies)
        {
            enemies.OnExplosionAttacked += EnemyExplosionAttacked;
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

    void EnemyExplosionAttacked(GameObject unit)
    {
        if (!showCombo)
        {
            showCombo = true;

            ShowReadMe(readme);
        }
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
