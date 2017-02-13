using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBattle : UnitBattle {
    [SerializeField]
    private GameObject AttackRegionObj;
    [SerializeField]
    private float HP;
    [SerializeField]
    private float nowHP;
    [SerializeField]
    private float recoverTime;
    [SerializeField]
    private AudioClip guardSE;
    [SerializeField]
    private AudioClip attackSE;
    [SerializeField]
    private float strength;
    [SerializeField]
    private float force;

    public bool Missing { get; set; }

    public static bool IsDead { get { return dead; } }
    private static bool dead;

    private Subject<Unit> attackSubject = new Subject<Unit>();
    public IObservable<Unit> OnAttack { get { return attackSubject; } }

    private Subject<Unit> deadSubject = new Subject<Unit>();
    public IObservable<Unit> OnDead { get { return deadSubject; } }

    private Subject<UnitBattle> attackedSubject = new Subject<UnitBattle>();
    public IObservable<UnitBattle> OnAttacked { get { return attackedSubject; } }

    private FloatReactiveProperty HPrate = new FloatReactiveProperty(1.0f);
    public IObservable<float> HPRate { get { return HPrate; } }

    Animator anim;

    float recoverStart;

    int attackHash;
    int slashEndHash;
    int EnemyMask;

    bool guardAttack = false;
    Vector3 AttackRange;
    PlayerSlash slash;
    TrailEffect trail;

    void Awake()
    {
        dead = false;

        slash = GetComponent<PlayerSlash>();
        
        if (AttackRegionObj)
        {
            AttackRange = AttackRegionObj.GetComponent<BoxCollider>().size / 2.0f;
        }

        trail = GetComponent<TrailEffect>();
    }

    void Start()
    {
        attackHash = Animator.StringToHash("PlayerBase.Attack");
        slashEndHash = Animator.StringToHash("PlayerBase.SlashEnd");
        EnemyMask = LayerMask.GetMask("Enemy");
        anim = GetComponent<Animator>();

        nowHP = HP;

        InputController.OnAttackClick.Subscribe(u => Attack()).AddTo(this);
        EnemyManager.OnEnemyClicked.Subscribe(o => Battle(o)).AddTo(this);
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        if (!enabled)
        {
            return;
        }

        if (Time.time - recoverStart > recoverTime)
        {
            ++nowHP;
            if (nowHP > HP)
            {
                nowHP = HP;
            }
            HPrate.Value = nowHP / HP;

            recoverStart = Time.time;
        }

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash != attackHash)
        {
            trail.AttackTrailEnd();
        }
    }

    void Attack()
    {
        if (!enabled)
        {
            return;
        }

        if (slash && slash.IsSlashing)
        {
            return;
        }

        GameObject obj = null;
        float angle = Mathf.Infinity;
        Collider[] cs = Physics.OverlapSphere(transform.position, 1.5f, EnemyMask);
        cs.ToObservable().Subscribe(c =>
        {
            EnemyBattle enemy = c.GetComponent<EnemyBattle>();
            if (enemy)
            {
                Vector3 dir = enemy.gameObject.transform.position - transform.position;
                float tmp = Mathf.Abs(Vector3.Angle(dir, transform.forward));
                if (tmp < angle)
                {
                    angle = tmp;
                    obj = enemy.gameObject;
                }
            }
        });

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash != slashEndHash)
        {
            AttackEnemy(obj);
        }
    }

    void Battle (GameObject Enemy)
    {
        YellowDebuff[] yellow = GameObject.FindObjectsOfType<YellowDebuff>();
        yellow.ToObservable().Subscribe(y => 
        {
            if (y.vistom.gameObject == Enemy)
            {
                y.End();
                return;
            }
        });

        //if (CanAttack(Enemy))
        {
            //AttackEnemy(Enemy);
        }
    }

    bool CanAttack(GameObject Enemy)
    {
        if (dead)
        {
            return false;
        }

        if (slash && slash.SlashEnemy(Enemy))
        {
            return false;
        }

        return true;
    }   

	void AttackEnemy(GameObject enemy)
	{
        if (enemy != null)
        {
            Vector3 pos = enemy.transform.position;
            pos.y = 0f;
            transform.LookAt(pos);
        }

        anim.SetTrigger("Attack");

        attackSubject.OnNext(Unit.Default);
    }

    void AttackStart()
    {
        AudioHelper.PlaySE(gameObject, attackSE);
        trail.AttackTrailStart();
    }

    void AttackEnd()
    {
        trail.AttackTrailEnd();
    }

    void AttackHit()
    {
        float power = (GetComponent<PlayerAttribute>().Type == ElementType.ELEMENT_TYPE_NONE) ? strength : strength + 1;

        Collider[] cs = Physics.OverlapBox(AttackRegionObj.transform.position, AttackRange, AttackRegionObj.transform.rotation, EnemyMask);
        cs.ToObservable().Subscribe(c =>
        {
            EnemyBattle enemy = c.GetComponent<EnemyBattle>();
            if (enemy)
            {
                enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_NORMAL, power, force));
            }
        });
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        if (!enabled)
        {
            return false;
        }
        
        if (slash && slash.IsSlashing)
        {
            return false;
        }

        AddForce((transform.position - unit.transform.position).normalized * attack.Force);

        if (anim.GetBool("Guard") || guardAttack)
        {
            if (attack.Type == AttackType.ATTACK_TYPE_NORMAL)
            {
                AudioHelper.PlaySE(gameObject, guardSE);
                guardAttack = true;

                unit.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_REFLECT, 3f));
            }

            return false;
        }

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (attack.Type == AttackType.ATTACK_TYPE_REFLECT && info.fullPathHash == attackHash)
        {
            anim.SetTrigger("AttackFail");

            anim.SetBool("UnNormal", true);
            return false;
        }

        if (attack.Strength <= 0)
        {
            return false;
        }

        attackedSubject.OnNext(unit);

        recoverStart = Time.time;
        nowHP -= attack.Strength;

        HPrate.Value = nowHP / HP;

        Vector3 pos = unit.transform.position;
        pos.y = 0f;
        transform.LookAt(pos);

        if (nowHP > 0)
        {
            anim.SetTrigger("Hurt");
        }

        ShakeCamera shake = GameObject.FindObjectOfType<ShakeCamera>();
        if (shake)
        {
            shake.enabled = true;
        }

        if (nowHP <= 0)
        {
            anim.SetTrigger("Die");
            anim.SetBool("Attack", false);
            anim.SetBool("Skill", false);
            anim.SetBool("Hurt", false);
            anim.SetBool("Slash", false);
            enabled = false;
            dead = true;

            deadSubject.OnNext(Unit.Default);
        }

        return true;
    }

    public void Revive()
    {
        enabled = true;
        dead = false;
        nowHP = HP;

        anim.Play("PlayerBase.Idle");

        anim.SetBool("IsMove", false);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
