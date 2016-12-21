using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BasicStage : MonoBehaviour {
    [SerializeField]
    private GameObject opening;
    [SerializeField]
    private GameObject attack;
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
    private GameObject attri;
    [SerializeField]
    private GameObject exit;
    [SerializeField]
    private ImmunityAura blueAura;
    [SerializeField]
    private BlockAttackAura greenAura;
    [SerializeField]
    private RoundDamageAura redAura;
    [SerializeField]
    private GameObject red;
    [SerializeField]
    private GameObject green;
    [SerializeField]
    private GameObject blue;

    GameSystem system;

    System.IDisposable firstSlash;
    System.IDisposable waitSlash;
    System.IDisposable firstDie;
    System.IDisposable firstClear;
    System.IDisposable firstCharge;
    System.IDisposable firstSkill;
    System.IDisposable enemyHint;

    void Awake()
    {
        system = GameObject.FindObjectOfType<GameSystem>();

        if (blueAura)
        {
            blueAura.OnBlock.Subscribe(_ => ShowEnemyHit(blue)).AddTo(this);
        }

        if (redAura)
        {
            redAura.OnDamage.Subscribe(_ => ShowEnemyHit(red)).AddTo(this);
        }

        if (greenAura)
        {
            greenAura.OnBlock.Subscribe(_ => ShowEnemyHit(green)).AddTo(this);
        }
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
            firstCharge = skill.OnCharge.Subscribe(e => SkillStep()).AddTo(this);
            firstSkill = skill.OnSkill.Subscribe(e => ToExit()).AddTo(this);

            PlayerBattle battle = skill.gameObject.GetComponent<PlayerBattle>();
            if (battle)
            {
                battle.OnDead.Subscribe(_ => Observable.Timer(System.TimeSpan.FromSeconds(3f)).Subscribe(t => battle.Revive())).AddTo(this);
            }
        }
    }
	
    void CheckSlash()
    {
        if (slash && slash.activeSelf && Input.GetKeyDown(KeyCode.S) && system)
        {
            system.GameResume();
            waitSlash.Dispose();

            PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
            if (player)
            {
                player.Slash();
            }
        }
    }

    void OnOpeningEnd()
    {
        if (system)
        {
            system.GamePause();
        }

        if (opening)
        {
            opening.SetActive(true);
        }
    }

    public void StageStart()
    {
        if (system)
        {
            system.GameResume();
        }

        if (opening)
        {
            opening.SetActive(false);
        }

        if (attack)
        {
            attack.SetActive(true);
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

        if (attack)
        {
            attack.SetActive(false);
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

        if (attri)
        {
            attri.SetActive(true);
        }

        if (energyHint)
        {
            energyHint.SetActive(false);
        }

        firstCharge.Dispose();
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

        firstSkill.Dispose();
    }

    void ShowEnemyHit(GameObject obj)
    {
        if (enemyHint != null)
        {
            enemyHint.Dispose();
        }

        if (red)
        {
            red.SetActive(false);
        }

        if (blue)
        {
            blue.SetActive(false);
        }

        if (green)
        {
            green.SetActive(false);
        }

        if (obj)
        {
            obj.SetActive(true);
            enemyHint = Observable.Timer(System.TimeSpan.FromSeconds(3f)).Subscribe(_ => obj.SetActive(false));
        }
    }
}
