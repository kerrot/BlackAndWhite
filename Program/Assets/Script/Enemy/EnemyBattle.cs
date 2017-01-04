using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyBattle : UnitBattle
{
    [SerializeField]
    protected float deadTime;
    [SerializeField]
    protected AudioClip tumbleSE;
    [SerializeField]
    protected AudioClip fireSE;
    [SerializeField]
    protected AudioClip woodSE;
    [SerializeField]
    protected AudioClip frightenSE;
    [SerializeField]
    protected GameObject wanderEffect;
    [SerializeField]
    protected GameObject energyPeace;
    [SerializeField]
    private Transform textUICenter;
    [SerializeField]
    protected int peaceCount = 2;

    protected Subject<Unit> attackedSubject = new Subject<Unit>();
    protected Subject<GameObject> dieSubject = new Subject<GameObject>();
    protected Subject<GameObject> explosionAttacked = new Subject<GameObject>();

    public IObservable<GameObject> OnDie { get { return dieSubject; } }
    public IObservable<GameObject> OnExplosionAttacked { get { return explosionAttacked; } }
    public IObservable<Unit> OnAttacked { get { return attackedSubject; } }

    public GameObject DeadAction;

    protected EnemySlash slash;
    protected EnemyHP HPState;

    protected Animator anim;

    protected float deadStart;

    protected Collider coll;

    protected int wanderHash;
    protected int damageHash;

    bool dead;
    protected PlayerBattle player;
    protected Attribute attr;

    GameObject blockUI;

    //Start change to Awake, because Instantiate not call Start but Awake
    void Awake()
    {
        attr = GetComponent<Attribute>();

        player = GameObject.FindObjectOfType<PlayerBattle>();
        coll = GetComponent<Collider>();
        slash = GetComponent<EnemySlash>();
        HPState = GetComponent<EnemyHP>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        wanderHash = Animator.StringToHash("EnemyBase.Wander");
        damageHash = Animator.StringToHash("EnemyBase.DamageStart");

        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            blockUI = ui.CreateBlockUI();
            blockUI.SetActive(false);
        }

        EnemyManager manager = GameObject.FindObjectOfType<EnemyManager>();
        if (manager)
        {
            manager.AddMonster(gameObject);
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
        this.OnAnimatorMoveAsObservable().Subscribe(_ => UniRxAnimatorMove());
    }

    void UniRxAnimatorMove()
    {
        transform.position = anim.rootPosition;
    }

    void UniRxUpdate()
    {
        if (HPState && HPState.HP.Value <= 0 && Time.time - deadStart > deadTime)
        {
            HPState.Revive();
            coll.enabled = true;
            anim.SetTrigger("Revive");
        }


        if (player && wanderEffect)
        {
            anim.SetBool("Wander", player.Missing);
            wanderEffect.SetActive(player.Missing);
        }

        if (blockUI.activeSelf)
        {
            blockUI.transform.position = Camera.main.WorldToScreenPoint(textUICenter.transform.position);
        }
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        if (dead)
        {
            return false;
        }

        attackedSubject.OnNext(Unit.Default);

        #region Modify with Attribute
        Attribute attr = GetComponent<Attribute>();
        if (attr && attr.ProcessAttack(unit, attack))
        {
            blockUI.SetActive(false);
            blockUI.SetActive(true);
            return false;
        }
        #endregion

        bool physics = true;

        if (unit == player)
        {
            transform.LookAt(player.transform);
        }

        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && slash.CanSlash)
        {
            Die(attack);
        }
        else if (attack.Type == AttackType.ATTACK_TYPE_EXPLOSION)
        {
            if (slash.CanSlash)
            {
                //anim.SetTrigger("Hitted");
                //GetComponent<Collider>().enabled = false;

                //Observable.FromCoroutine(_ => LateDie(attack)).Subscribe().AddTo(this);
                slash.TriggerSlash();
                physics = false;
            }
            else
            {
                HPState.Barrier.Value = 0;
                slash.TriggerSlash();
                explosionAttacked.OnNext(gameObject);
            }
        }
        else if (HPState.Barrier.Value <= 0)
        {
            HPState.HP.Value -= attack.Strength;
            if (HPState.HP.Value <= 0)
            {
                coll.enabled = false;
                anim.SetTrigger("Die");
                deadStart = Time.time;
            }
            else
            {
                anim.SetTrigger("Hitted");
            }
        }
        else
        {
            HPState.Barrier.Value -= attack.Strength;
            if (HPState.Barrier.Value > 0)
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

        if (physics)
        {
            Vector3 direction = transform.position - unit.transform.position;
            AddForce(direction.normalized * attack.Force);
        }

        return true;
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
                act.Atk = new Attack() { Type = AttackType.ATTACK_TYPE_EXPLOSION,
                                        Element = attr && Attribute.isBase(attr.Type) ? attr.Type : attack.Element,
										Strength = 5f, Force = 50000};
            }
        }

        
        if (attr && Attribute.isBase(attr.Type))
        {
            ProduceEnergyPeace(attr.Type, peaceCount);
        }
        else
        {
            if ((attack.Element & ElementType.ELEMENT_TYPE_RED) != 0)
            {
                ProduceEnergyPeace(ElementType.ELEMENT_TYPE_RED, peaceCount / 2);
            }
            if ((attack.Element & ElementType.ELEMENT_TYPE_GREEN) != 0)
            {
                ProduceEnergyPeace(ElementType.ELEMENT_TYPE_GREEN, peaceCount / 2);
            }
            if ((attack.Element & ElementType.ELEMENT_TYPE_BLUE) != 0)
            {
                ProduceEnergyPeace(ElementType.ELEMENT_TYPE_BLUE, peaceCount / 2);
            }
        }

        dead = true;

        dieSubject.OnNext(gameObject);
    }

    void ProduceEnergyPeace(ElementType ele, int num)
    {
        if (energyPeace)
        {
            for (int i = 0; i < num; ++i)
            {
                GameObject obj = Instantiate(energyPeace, transform.position, Quaternion.identity) as GameObject;
                EnergyPeace peace = obj.GetComponent<EnergyPeace>();
                if (peace)
                {
                    peace.Type = ele;
                }
            }
        }
    }

    IEnumerator LateDie(Attack attack)
    {
        yield return new WaitForSeconds(0.5f);
        Die(attack);
    }

    void UniRxOnDestroy()
    {
		if (transform.parent != null) {
			Destroy (transform.parent.gameObject);
		}
    }
}