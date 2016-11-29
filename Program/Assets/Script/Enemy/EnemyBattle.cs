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
    private float deadTime;
    [SerializeField]
    private float showHPTime;
    [SerializeField]
    private Transform HPUICenter;
    [SerializeField]
    private AudioClip tumbleSE;
    [SerializeField]
    private AudioClip fireSE;
    [SerializeField]
    private AudioClip woodSE;
    [SerializeField]
    private AudioClip frightenSE;

    private Subject<GameObject> dieSubject = new Subject<GameObject>();
    private Subject<GameObject> explosionAttacked = new Subject<GameObject>();

    public IObservable<GameObject> OnDie { get { return dieSubject; } }
    public IObservable<GameObject> OnExplosionAttacked { get { return explosionAttacked; } }

    public GameObject DeadAction;
    public Vector3 DeadEffectOffset;

    EnemySlash slash;

    HPBarUI hpUI;
    float showHPStart;

    Animator anim;
    
    float currentBarrier;
    float currentHP;
    float currentRecover;
    float deadStart;

    Collider coll;

    int wanderHash;
    int damageHash;

    //Start change to Awake, because Instantiate not call Start but Awake
    void Awake()
    {
        coll = GetComponent<Collider>();
        slash = GetComponent<EnemySlash>();
        anim = GetComponent<Animator>();
        currentBarrier = barrierStrength;
        currentHP = HPMax;
        currentRecover = 0;
    }

    void Start()
    {
        wanderHash = Animator.StringToHash("EnemyBase.Wander");
        damageHash = Animator.StringToHash("EnemyBase.DamageStart");

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
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
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

        if (currentHP <= 0 && Time.time - deadStart > deadTime)
        {
            currentHP = HPMax;
            coll.enabled = true;
            anim.SetTrigger("Revive");
            hpUI.SetBarrierEnable(true);
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
        #region ShowHP
        hpUI.gameObject.SetActive(true);
        showHPStart = Time.time;
        #endregion

        #region Modify with Attribute
        Attribute attr = GetComponent<Attribute>();
        if (attr && attr.ProcessAttack(unit, attack))
        {
            return false;
        }
        #endregion

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

                Observable.FromCoroutine(_ => LateDie(attack)).Subscribe().AddTo(this);
            }
            else
            {
                currentBarrier = 0;
                slash.TriggerSlash();
                explosionAttacked.OnNext(gameObject);                
            }
        }
        else if (currentBarrier <= 0)
        {
            currentHP -= attack.Strength;
            if (currentHP <= 0)
            {
                coll.enabled = false;
                anim.SetTrigger("Die");
                deadStart = Time.time;
                hpUI.SetBarrierEnable(false);
            }
        }
        else
        {
            currentBarrier -= attack.Strength;
            if (currentBarrier > 0)
            {
                if (attack.Type == AttackType.ATTACK_TYPE_SKILL)
                {
                    if (attack.Element == ElementType.ELEMENT_TYPE_BLUE)
                    {
                        anim.SetTrigger("Tumble");
                        AudioHelper.PlaySE(gameObject, tumbleSE);
                    }
                    else if (attack.Element == ElementType.ELEMENT_TYPE_RED)
                    {
                        anim.SetTrigger("Fire");
                        AudioHelper.PlaySE(gameObject, fireSE);
                    }
                    else if (attack.Element == ElementType.ELEMENT_TYPE_GREEN)
                    {
                        anim.SetTrigger("Hitted");
                        AudioHelper.PlaySE(gameObject, woodSE);
                    }
                    else if (attack.Element == ElementType.ELEMENT_TYPE_YELLOW)
                    {
                        anim.SetTrigger("DamageStart");
                    }
                    else
                    {
                        anim.SetTrigger("Hitted");
                    }
                }
                else
                {
                    AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                    if (info.fullPathHash == wanderHash)
                    {
                        anim.SetTrigger("Frighten");
                        AudioHelper.PlaySE(gameObject, frightenSE);
                    }
                    else
                    {
                        anim.SetTrigger("Hitted");
                    }
                }
            }
            else
            {
                slash.TriggerSlash();
            }
        }

        return false;
    }

    public void RecoverFromDamage()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash == damageHash)
        {
            anim.SetTrigger("DamageEnd");
        }
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

        dieSubject.OnNext(gameObject);
    }

    IEnumerator LateDie(Attack attack)
    {
        yield return new WaitForSeconds(0.5f);
        Die(attack);
    }

    void UniRxOnDestroy()
    {
        if (hpUI)
        {
            Destroy(hpUI.gameObject);
        }

		if (transform.parent != null) {
			Destroy (transform.parent.gameObject);
		}
    }
}