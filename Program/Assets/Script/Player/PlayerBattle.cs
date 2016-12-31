using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class PlayerBattle : UnitBattle {
    [SerializeField]
    private GameObject AttackRegionObj;
    [SerializeField]
    private Animator hurtEffect;
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

    Animator anim;

    float recoverStart;
    float cuurentIntensity;

    int attackHash;
    int EnemyMask;

    bool guardAttack = false;
    Vector3 AttackRange;
    PlayerSlash slash;

    void Awake()
    {
        dead = false;

        slash = GetComponent<PlayerSlash>();
        if (hurtEffect)
        {
            GameObject tmp = GameObject.Find("HurtScreenEffect");
            if (tmp)
            {
                hurtEffect = tmp.GetComponent<Animator>();
            }
        }
        
        if (AttackRegionObj)
        {
            AttackRange = AttackRegionObj.GetComponent<BoxCollider>().size / 2.0f;
        }
    }

    void Start()
    {
        attackHash = Animator.StringToHash("PlayerBase.Attack");
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

            recoverStart = Time.time;

            UpdateCameraEffect();
        }

        VignetteAndChromaticAberration effect = Camera.main.gameObject.GetComponent<VignetteAndChromaticAberration>();
        if (effect && effect.intensity != cuurentIntensity)
        {
            effect.intensity += (cuurentIntensity > effect.intensity) ? 0.001f : -0.001f;
        }
    }

    void Attack()
    {
        if (!enabled)
        {
            return;
        }

        GameObject obj = EnemyManager.GetEnemyByMousePosition(Input.mousePosition);
        AttackEnemy(obj);
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

        if (CanAttack(Enemy))
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
            transform.LookAt(enemy.transform);
        }

        anim.SetTrigger("Attack");

        attackSubject.OnNext(Unit.Default);
    }

    void AttackStart()
    {
        AudioHelper.PlaySE(gameObject, attackSE);
    }

    void AttackHit()
    {
        Collider[] cs = Physics.OverlapBox(AttackRegionObj.transform.position, AttackRange, AttackRegionObj.transform.rotation, EnemyMask);
        cs.ToObservable().Subscribe(c =>
        {
            EnemyBattle enemy = c.GetComponent<EnemyBattle>();
            if (enemy)
            {
                enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_NORMAL, strength, force));
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

        recoverStart = Time.time;
        nowHP -= attack.Strength;

        if (nowHP > 0)
        {
            anim.SetTrigger("Hurt");
        }
        
        if (attack.Strength > 0)
        {
            hurtEffect.SetTrigger("Play");
        }

        //ShakeCamera shake = GameObject.FindObjectOfType<ShakeCamera>();
        //if (shake)
        //{
        //    shake.enabled = true;
        //}

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

        UpdateCameraEffect();

        return true;
    }

    void UpdateCameraEffect()
    {
        if (nowHP < HP)
        {
            cuurentIntensity = 0.6f - (nowHP - 1f) / HP * 0.6f;
        }
    }

    public void Revive()
    {
        enabled = true;
        dead = false;
        nowHP = HP;

        cuurentIntensity = 0f;
        VignetteAndChromaticAberration effect = Camera.main.gameObject.GetComponent<VignetteAndChromaticAberration>();
        if (effect)
        {
            effect.intensity = 0f;
        }

        anim.Play("PlayerBase.Idle");
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
