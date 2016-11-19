using UniRx;
using UnityEngine;
using System.Collections;

public class TutorialStage : MonoBehaviour {
    [SerializeField]
    private GameObject opening;
    [SerializeField]
    private GameObject canSlash;
    [SerializeField]
    private GameObject explosion;    
    [SerializeField]
    private MenuControl menu;

    bool showSlash = false;
    bool showCombo = false;

    // Use this for initialization
    void Start () {
        CloseReadme();

        OpeningRTM opening = GameObject.FindObjectOfType<OpeningRTM>();
        if (opening)
        {
			opening.OnOpeningEnd.Subscribe(o => OnOpeningEnd());
        }

        EnemyGenerator enemies = GameObject.FindObjectOfType<EnemyGenerator>();
        if (enemies)
        {
            enemies.OnExplosionAttacked += EnemyExplosionAttacked;
            enemies.OnEnemyCanSlash += EnemySlashTriggered;
			enemies.OnEnemyEmpty.Subscribe(o => StageClear());
        }
	}
	
	void OnOpeningEnd()
    {
        if (opening)
        {
            ShowReadMe(opening);
        }
        else
        {
            Resume();
        }
    }

    public void Resume()
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GameResume();
        }

        CloseReadme();
    }

    void EnemyExplosionAttacked(GameObject unit)
    {
        if (explosion && !showCombo)
        {
            showCombo = true;

            ShowReadMe(explosion);
        }
    }

    void EnemySlashTriggered(GameObject unit)
    {
        if (canSlash && !showSlash)
        {
            showSlash = true;

            ShowReadMe(canSlash);
        }
    }

    void ShowReadMe(GameObject obj)
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GamePause();
        }

        if (obj)
        {
            obj.SetActive(true);
        }
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

    void CloseReadme()
    {
        if (opening)
        {
            opening.SetActive(false);
        }

        if (canSlash)
        {
            canSlash.SetActive(false);
        }

        if (explosion)
        {
            explosion.SetActive(false);
        }
    }
}
