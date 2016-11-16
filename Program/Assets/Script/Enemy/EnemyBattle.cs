using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyBattle : UnitBattle
{
    [SerializeField]
    private float HPMax;
    [SerializeField]
    private float barrierStrength;
    [SerializeField]
    private float recoverTime;
    [SerializeField]
    private float showHPTime;
    [SerializeField]
    private Transform HPUICenter;
    [SerializeField]
    private AudioClip tumbleSE;

    public UnitAction OnDie;
    public UnitAction OnExplosionAttacked;

    public GameObject DeadAction;
    public Vector3 DeadEffectOffset;

    EnemySlash slash;

    HPBarUI hpUI;
    float showHPStart;

    Animator anim;
    
    float currentBarrier;
    float currentHP;
    float currentRecover;

    //Start change to Awake, because Instantiate not call Start but Awake
    void Awake()
    {
        slash = GetComponent<EnemySlash>();
        anim = GetComponent<Animator>();
        currentBarrier = barrierStrength;
        currentHP = HPMax;
        currentRecover = 0;
    }

    void Start()
    {
        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        { 
            GameObject tmp = ui.CreateHPUI();
            hpUI = tmp.GetComponent<HPBarUI>();
            hpUI.SetHPMax(HPMax);
            hpUI.SetHPCurrent(currentHP);
            hpUI.SetBarrierMax(barrierStrength);
            hpUI.SetBarrierCurrent(currentBarrier);
            hpUI.SetRecoverMax(recoverTime);
            hpUI.SetRecoverCurrent(currentRecover);
            hpUI.SetRecoverEnable(false);
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());       
    }

    void UniRxUpdate()
    {
        if (hpUI.gameObject.activeSelf)
        {
            hpUI.transform.position = Camera.main.WorldToScreenPoint(HPUICenter.transform.position);

            if (currentBarrier <= 0)
            {
                hpUI.SetRecoverEnable(true);
                currentRecover += Time.deltaTime;
                if (currentRecover > recoverTime)
                {
                    currentRecover = 0;
                    showHPStart = Time.time;
                    hpUI.SetRecoverEnable(false);
                    currentBarrier = barrierStrength;
                }
            }

            if (currentRecover == 0 && Time.time - showHPStart > showHPTime)
            {
                hpUI.gameObject.SetActive(false);
            }

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        hpUI.SetHPCurrent(currentHP);
        hpUI.SetBarrierCurrent(currentBarrier);
        hpUI.SetRecoverCurrent(currentRecover);
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        hpUI.gameObject.SetActive(true);
        showHPStart = Time.time;

        Immunity im = GetComponent<Immunity>();
        if (im && im.CheckImmunity(unit, attack))
        {
            return false;
        }

        Attribute attr = GetComponent<Attribute>();
        if (attr && attr.ProcessAttack(unit, attack))
        {
            return false;
        }

        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && slash.CanSlash)
        {
            Die(attack);
            return true;
        }
        else if (attack.Type == AttackType.ATTACK_TYPE_EXPLOSION)
        {
            if (slash.CanSlash)
            {
                anim.SetTrigger("Hitted");
                GetComponent<Collider>().enabled = false;

                StartCoroutine(LateDie(attack));
            }
            else
            {
                currentBarrier = 0;
                slash.TriggerSlash();
                if (OnExplosionAttacked != null)
                {
                    OnExplosionAttacked(gameObject);
                }
            }
        }
        else
        {
            currentBarrier -= attack.Strength;
            if (currentBarrier > 0)
            {
                if (attack.Type == AttackType.ATTACK_TYPE_SKILL)
                {
                    anim.SetTrigger("Tumble");
                    PlaySE(tumbleSE);
                }
                else
                {
                    anim.SetTrigger("Hitted");
                }
            }
            else
            {
                slash.TriggerSlash();
            }
        }

        return false;
    }

    void Die(Attack attack)
    {
        if (DeadAction)
        {
            DeadAction act = DeadAction.GetComponent<DeadAction>();
            if (act)
            {
                act.Attacker = this;
                act.Atk = new Attack() { Type = AttackType.ATTACK_TYPE_EXPLOSION, Element = attack.Element };
            }
        }

        if (OnDie != null)
        {
            OnDie(gameObject);
        }
    }

    IEnumerator LateDie(Attack attack)
    {
        yield return new WaitForSeconds(0.5f);
        Die(attack);
    }

    void OnDestroy()
    {
        if (hpUI)
        {
            Destroy(hpUI.gameObject);
        }
    }
}