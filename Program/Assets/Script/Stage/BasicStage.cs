using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BasicStage : MonoBehaviour {
    [SerializeField]
    private GameObject opening;
    [SerializeField]
    private GameObject slash;
    [SerializeField]
    private EnemyBattle LV1Enemy;
    [SerializeField]
    private GameObject LV2;
    [SerializeField]
    private GameObject LV3;
    [SerializeField]
    private GameObject combo;
    [SerializeField]
    private GameObject skillUI;
    [SerializeField]
    private GameObject energyHint;
    [SerializeField]
    private GameObject skillHint;
    [SerializeField]
    private GameObject exit;

    GameSystem system;

    System.IDisposable firstSlash;
    System.IDisposable waitSlash;
    System.IDisposable firstDie;
    System.IDisposable firstClear;


    void Awake()
    {
        system = GameObject.FindObjectOfType<GameSystem>();
    }

    void Start ()
    {
        OpeningRTM opening = GameObject.FindObjectOfType<OpeningRTM>();
        if (opening)
        {
            opening.OnOpeningEnd.Subscribe(o => OnOpeningEnd()).AddTo(this);
        }

        firstSlash = EnemyManager.OnEnemyCanSlash.Subscribe(o => EnemySlashTriggered(o)).AddTo(this);
        firstDie = LV1Enemy.OnDie.Subscribe(o => Observable.FromCoroutine(ToLV2).Subscribe()).AddTo(this);

        PlayerSkill skill = GameObject.FindObjectOfType<PlayerSkill>();
        if (skill)
        {
            skill.OnCharge.Subscribe(e => SkillStep());
            skill.OnSkill.Subscribe(e => ToExit());
        }
    }
	
    void CheckSlash()
    {
        if (slash && slash.activeSelf && Input.anyKey && system)
        {
            system.GameResume();
            waitSlash.Dispose();
        }
    }

    void OnOpeningEnd()
    {
        if (system)
        {
            system.GameResume();
        }

        if (opening)
        {
            opening.SetActive(true);
        }
    }

    void EnemySlashTriggered(GameObject unit)
    {
        if (slash)
        {
            slash.SetActive(true);
        }

        if (system)
        {
            system.GamePause();
        }

        firstSlash.Dispose();
        waitSlash = this.UpdateAsObservable().Subscribe(_ => CheckSlash());
    }

    IEnumerator ToLV2()
    {
        firstDie.Dispose();
        
        yield return new WaitForSeconds(2f);
        if (LV2)
        {
            LV2.SetActive(true);
        }

        if (opening)
        {
            opening.SetActive(false);
        }

        if (slash)
        {
            slash.SetActive(false);
        }

        if (combo)
        {
            combo.SetActive(true);
        }

        firstClear = EnemyManager.OnEnemyEmpty.Subscribe(o => Observable.FromCoroutine(ToLV3).Subscribe()).AddTo(this);
        RegisterEnemy();
    }

    IEnumerator ToLV3()
    {
        firstClear.Dispose();
        yield return new WaitForSeconds(2f);

        if (LV3)
        {
            LV3.SetActive(true);
        }

        if (combo)
        {
            combo.SetActive(false);
        }

        if (skillUI)
        {
            skillUI.SetActive(true);
        }

        if (energyHint)
        {
            energyHint.SetActive(true);
        }

        RegisterEnemy();
    }

    void RegisterEnemy()
    {
        EnemyManager manager = GameObject.FindObjectOfType<EnemyManager>();
        if (manager)
        {
            GameObject.FindObjectsOfType<EnemyBattle>().ToList().ForEach(e => manager.AddMonster(e.gameObject));
        }
    }

    void SkillStep()
    {
        if (skillHint)
        {
            skillHint.SetActive(true);
        }

        if (energyHint)
        {
            energyHint.SetActive(false);
        }
    }

    void ToExit()
    {
        if (skillHint)
        {
            skillHint.SetActive(false);
        }

        if (exit)
        {
            exit.SetActive(true);
        }
    }
}
