using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class PlayerBattle : UnitBattle {
    public EnemyGenerator Enemies;
    [SerializeField]
    private GameObject AttackRegion;
    [SerializeField]
    private float AttackAngle = 60;
    [SerializeField]
    private Animator hurtEffect;
    [SerializeField]
    private float HP;
    [SerializeField]
    private float nowHP;
    [SerializeField]
    private float recoverTime;

    float AttackRadius = 1.3f;
    Animator anim;

    AudioSource swing;

    float recoverStart;
    float cuurentIntensity;

    int attackHash;

    void Start()
    {
        attackHash = Animator.StringToHash("PlayerBase.Attack");

        Enemies.OnEnemyClicked += Battle;
        anim = GetComponent<Animator>();
        AttackRadius = AttackRegion.GetComponent<SphereCollider>().radius;
        swing = GetComponent<AudioSource>();

        nowHP = HP;

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

    void Battle (GameObject Enemy)
    {
        if (!PlayerSlash.Instance.SlashEnemy(Enemy))
        {
            AttackEnemy(Enemy);
        }
    }

	void AttackEnemy(GameObject Enemy)
	{
		Vector3 direction = Enemy.transform.position - transform.position;
		if (direction.magnitude < AttackRadius) {
			PlayerMove.Instance.CanRotate = false;
			anim.SetTrigger("Attack");
            swing.Play();
        }
	}

    void AttackHit()
    {
        List<GameObject> list = Enemies.GetEnemy(transform.position, AttackRadius, transform.rotation * Vector3.forward, AttackAngle);
        list.ForEach(o =>
        {
            EnemyBattle Enemy = o.GetComponent<EnemyBattle>();
            Enemy.Attacked(this, new Attack() { Strength = 2f });
        });

        if (list.Count > 0)
        {
            GameSystem.Instance.Attack();
        }
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        recoverStart = Time.time;
        nowHP -= attack.Strength;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (attack.Type == AttackType.ATTACK_TYPE_REFLECT && info.fullPathHash == attackHash)
        {
            anim.SetTrigger("AttackFail");
            return false;
        }

        if (nowHP > 0)
        {
            anim.SetTrigger("Hurt");
        }
        
        hurtEffect.SetTrigger("Play");

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
