using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class PlayerBattle : UnitBattle {
    [SerializeField]
    private GameObject AttackRegion;
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

    public bool Missing { get; set; }

    private Subject<Unit> attackSubject = new Subject<Unit>();
    public IObservable<Unit> OnAttack { get { return attackSubject; } }

    Animator anim;

    float recoverStart;
    float cuurentIntensity;

    int attackHash;

    bool guardAttack = false;
    Vector3 guardPos;

    void Start()
    {
        attackHash = Animator.StringToHash("PlayerBase.Attack");

        anim = GetComponent<Animator>();

        nowHP = HP;

        InputController.OnAttackClick.Subscribe(u => Attack()).AddTo(this);
        EnemyGenerator.OnEnemyClicked.Subscribe(o => Battle(o)).AddTo(this);
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
		this.LateUpdateAsObservable().Subscribe (_ => UniRxLateUpdate ());
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


        if (guardAttack && transform.position != guardPos)
        {
            if (GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                guardPos = transform.position;
                anim.enabled = true;
            }
        }
    }

    void UniRxLateUpdate()
    {
        if (anim.enabled && guardAttack)
        {
            guardAttack = false;
            transform.position = guardPos;
        }
    }

    void Attack()
    {
        GameObject obj = EnemyGenerator.GetEnemyByMousePosition(Input.mousePosition);
        AttackEnemy(obj);
    }

    void Battle (GameObject Enemy)
    {
        if (CanAttack(Enemy))
        {
            AttackEnemy(Enemy);
        }
    }

    bool CanAttack(GameObject Enemy)
    {
        PlayerSlash slash = GetComponent<PlayerSlash>();
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

        PlayerMove move = GetComponent<PlayerMove>();
        if (move)
        {
            move.CanRotate = false;
        }
        anim.SetTrigger("Attack");

        AudioHelper.PlaySE(gameObject, attackSE);

        attackSubject.OnNext(Unit.Default);
    }

    void AttackHit()
    {
        Collider[] cs = Physics.OverlapBox(AttackRegion.transform.position, AttackRegion.GetComponent<BoxCollider>().size, AttackRegion.transform.rotation);
        cs.ToObservable().Subscribe(c =>
        {
            EnemyBattle enemy = c.GetComponent<EnemyBattle>();
            if (enemy)
            {
                enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_NORMAL, strength));
            }
        });
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        if (!enabled)
        {
            return false;
        }

        if (anim.GetBool("Guard") || guardAttack)
        {
            if (attack.Type == AttackType.ATTACK_TYPE_NORMAL)
            {
                AudioHelper.PlaySE(gameObject, guardSE);
                anim.enabled = false;
                guardAttack = true;
                guardPos = transform.position;

                Vector3 force = (transform.position - unit.transform.position).normalized;
                Rigidbody rd = GetComponent<Rigidbody>();
                rd.velocity = force * attack.Force;

                unit.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_REFLECT, 3f));
            }

            return false;
        }

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (attack.Type == AttackType.ATTACK_TYPE_REFLECT && info.fullPathHash == attackHash)
        {
            anim.SetTrigger("AttackFail");
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
            GameSystem system = GameObject.FindObjectOfType<GameSystem>();
            if (system)
            {
                system.GameOver();
                anim.SetTrigger("Die");

                enabled = false;
            }
        }

        UpdateCameraEffect();

        return false;
    }

    void UpdateCameraEffect()
    {
        if (nowHP < HP / 2)
        {
            cuurentIntensity = 0.7f - (nowHP - 1f) / HP * 2f * 0.7f;
        }
    }
}
