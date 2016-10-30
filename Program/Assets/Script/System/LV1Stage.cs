using UnityEngine;
using System.Collections;

public class LV1Stage : MonoBehaviour {
    [SerializeField]
    private GameObject readme1;
    [SerializeField]
    private GameObject readme2;
    [SerializeField]
    private MenuControl menu;

    bool showSlash = false;

    // Use this for initialization
    void Start () {
        readme1.SetActive(false);
        readme2.SetActive(false);

        OpeningRTM opening = GameObject.FindObjectOfType<OpeningRTM>();
        if (opening)
        {
            opening.OnOpeningEnd += ShowReadMe1;
        }

        EnemyGenerator enemies = GameObject.FindObjectOfType<EnemyGenerator>();
        if (enemies)
        {
            enemies.OnEnemyCanSlash += EnemySlashTriggered;
            enemies.OnEnemyEmpty += StageClear;
        }
	}
	
	void ShowReadMe1()
    {
        ShowReadMe(readme1);
    }

    public void Resume()
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GameResume();
        }

        readme1.SetActive(false);
        readme2.SetActive(false);
    }

    void EnemySlashTriggered(GameObject unit)
    {
        if (!showSlash)
        {
            showSlash = true;

            ShowReadMe(readme2);
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
        menu.gameObject.SetActive(true);
        menu.StageClear();
    }
}
