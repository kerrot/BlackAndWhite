﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;
using System.Collections;

public class KnightBattle : EnemyBattle {
    [SerializeField]
    private CorePeace peace;    // boss core
    [SerializeField]
    private GameObject smoke;    // land effect

    private Subject<Unit> reviveSubject = new Subject<Unit>();
    public IObservable<Unit> OnRevive { get { return reviveSubject; } }

    void Awake()
    {
        attr = GetComponent<Attribute>();

        player = GameObject.FindObjectOfType<PlayerBattle>();
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        HPState = GetComponent<EnemyHP>();
        if (HPState)
        {
            // peace union event, after first revive
            HPState.OnRecover.Subscribe(_ => reviveSubject.OnNext(Unit.Default)).AddTo(this);
            if (HPState.CanRecover)
            {
                peace.Register();
            }
        }

        anim.SetTrigger("TelePort");
    }

    void Start()
    {
        wanderHash = Animator.StringToHash("EnemyBase.Wander");

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
        this.OnAnimatorMoveAsObservable().Subscribe(_ => UniRxAnimatorMove());
    }

    void UniRxUpdate()
    {
        if (player && wanderEffect)
        {
            anim.SetBool("Wander", player.Missing && coll.enabled);
            wanderEffect.SetActive(player.Missing && coll.enabled);
        }
    }

    // no barrier
    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        if (!coll.enabled)
        {
            return false;
        }

        attackedSubject.OnNext(Unit.Default);

        tmpAtk.Type = attack.Type;
        tmpAtk.Strength = attack.Strength;
        tmpAtk.Force = attack.Force;
        tmpAtk.Element = attack.Element;

        Attribute attr = GetComponent<Attribute>();
		if (attr && attr.ProcessAttack(unit, tmpAtk))
		{
			return false;
		}

		if (HPState)
		{
			HPState.Barrier.Value -= tmpAtk.Strength;
			if (HPState.Barrier.Value <= 0) {
				Die ();
			}
            else
            {
                anim.SetTrigger("Hitted");
            }
        }

        return false;
    }

    void Die()
    {
        coll.enabled = false;
        anim.SetTrigger("Die");
        anim.SetBool("Hitted", false);

        if (peace)
        {
            peace.gameObject.SetActive(true);
        }
    }

    public void Revive()
    {
        anim.SetTrigger("Revive");
        anim.SetTrigger("TelePort");
        if (HPState)
        {
            HPState.Revive();
        }
    }

    void UniRxOnDestroy()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    // call the animation
    void Landing()
    {
        GameObject obj = Instantiate(smoke, transform.position, Quaternion.identity);
        obj.transform.parent = transform.parent;
        coll.enabled = true;
    }

    // call the animation
    void Jump()
    {
        coll.enabled = false;
    }
}
