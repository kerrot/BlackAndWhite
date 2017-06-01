using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyBattle : UnitBattle
{
    [SerializeField]
    protected float deadTime;               //revive time when dead
    [SerializeField]
    protected AudioClip tumbleSE;
    [SerializeField]
    protected AudioClip fireSE;
    [SerializeField]
    protected AudioClip woodSE;
    [SerializeField]
    protected AudioClip frightenSE;
    [SerializeField]
    protected GameObject wanderEffect;      //ui effect when player missing
    [SerializeField]
    protected GameObject energyPeace;       //drop item when dead
    [SerializeField]
    private Transform textUICenter;         //the position, break, block ui
    [SerializeField]
    protected ParticleSystem hitEffect;     //effect when being attacked
    [SerializeField]
    protected int peaceCount = 2;
    [SerializeField]
    private float explosionStrength = 5f;
    [SerializeField]
    private float explosionForce = 50000f;

    protected Subject<Unit> attackedSubject = new Subject<Unit>();
    protected Subject<GameObject> dieSubject = new Subject<GameObject>();
    // when break
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
    //protected int fireHash;

    bool dead;
    protected PlayerBattle player;
    protected Attribute attr;
    Vector3 animePos;
    GameObject blockUI;
    EnemyManager manager;

    protected Attack tmpAtk = new Attack();

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

        //block 
        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            blockUI = ui.CreateBlockUI();
            blockUI.SetActive(false);
        }

        // Register
        manager = GameObject.FindObjectOfType<EnemyManager>();
        if (manager)
        {
            manager.AddMonster(gameObject);
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
        this.OnAnimatorMoveAsObservable().Subscribe(_ => UniRxAnimatorMove());
    }

    // for root motion reason, prevent float
    protected void UniRxAnimatorMove()
    {
        animePos = anim.rootPosition;
        animePos.y = 0;
        transform.position = animePos;
    }

    void UniRxUpdate()
    {
        // chaeck revive
        if (HPState && HPState.HP.Value <= 0 && Time.time - deadStart > deadTime)
        {
            HPState.Revive();
            coll.enabled = true;
            anim.SetTrigger("Revive");
        }

        // player invisible
        if (player && wanderEffect)
        {
            anim.SetBool("Wander", player.Missing);
            wanderEffect.SetActive(player.Missing);
        }

        // follow the owner corresponding to screen
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

        tmpAtk.Type = attack.Type;
        tmpAtk.Strength = attack.Strength;
        tmpAtk.Force = attack.Force;
        tmpAtk.Element = attack.Element;

        #region Modify attack result by Attribute
        Attribute attr = GetComponent<Attribute>();
        if (attr && attr.ProcessAttack(unit, tmpAtk))
        {
            if (tmpAtk.Type == AttackType.ATTACK_TYPE_SLASH)
            {
                slash.CancelSlash();
            }

            blockUI.SetActive(false);
            blockUI.SetActive(true);
            return false;
        }
        #endregion

        // attacked effect, no matter damage or not
        if (hitEffect)
        {
            GameObject obj = Instantiate(hitEffect.gameObject, transform.position, hitEffect.transform.rotation);
            obj.transform.parent = transform.parent;
        }

        bool physics = true;

        // loot at player
        if (unit == player)
        {
            Vector3 pos = player.transform.position;
            pos.y = 0f;
            transform.LookAt(pos);
        }

        //real dead
        if (tmpAtk.Type == AttackType.ATTACK_TYPE_SLASH && slash.CanSlash)
        {
            Die(tmpAtk);
        }
        else if (tmpAtk.Type == AttackType.ATTACK_TYPE_EXPLOSION)
        {
            // break again but not apply physics
            if (slash.CanSlash)
            {
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
            HPState.HP.Value -= tmpAtk.Strength;
            if (HPState.HP.Value <= 0)
            {
                //will revive
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
            HPState.Barrier.Value -= tmpAtk.Strength;
            if (HPState.Barrier.Value > 0)
            {
                // abnormal state
                if (tmpAtk.Type == AttackType.ATTACK_TYPE_SKILL)
                {
                    if (tmpAtk.Element == ElementType.ELEMENT_TYPE_BLUE)
                    {
                        anim.SetTrigger("Tumble");
                        AudioHelper.PlaySE(gameObject, tumbleSE);
                    }
                    else if (tmpAtk.Element == ElementType.ELEMENT_TYPE_RED)
                    {
                        anim.SetTrigger("Fire");
                        AudioHelper.PlaySE(gameObject, fireSE);
                    }
                    else if (tmpAtk.Element == ElementType.ELEMENT_TYPE_GREEN)
                    {
                        anim.SetTrigger("Hitted");
                        AudioHelper.PlaySE(gameObject, woodSE);
                    }
                    else if (tmpAtk.Element == ElementType.ELEMENT_TYPE_YELLOW)
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
                    // attacked when player missing
                    AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                    if (info.fullPathHash == wanderHash)
                    {
                        anim.SetTrigger("Frighten");
                        AudioHelper.PlaySE(gameObject, frightenSE);
                    }

                    physics = false;
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
            AddForce(direction.normalized * tmpAtk.Force);
        }

        return true;
    }

    // for custom hurt animation
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
										Strength = explosionStrength, Force = explosionForce};
            }
        }

        //drop EnergyPeace base on the element type of attack
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
                obj.transform.parent = manager.transform;
                EnergyPeace peace = obj.GetComponent<EnergyPeace>();
                if (peace)
                {
                    peace.Type = ele;
                }
            }
        }
    }

    void UniRxOnDestroy()
    {
		if (transform.parent != null)
        {
			Destroy (transform.parent.gameObject);
		}

        Destroy(blockUI);
    }
}