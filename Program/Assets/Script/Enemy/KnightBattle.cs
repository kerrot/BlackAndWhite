using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class KnightBattle : EnemyBattle {


    void Awake()
    {
        attr = GetComponent<Attribute>();

        player = GameObject.FindObjectOfType<PlayerBattle>();
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        HPState = GetComponent<EnemyHP>();
        if (HPState)
        {
            HPState.OnRecover.Subscribe(_ => Revive()).AddTo(this);
        }
    }

    void Start()
    {
        wanderHash = Animator.StringToHash("EnemyBase.Wander");

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
        if (player)
        {
            anim.SetBool("Wander", player.Missing);
            wanderEffect.SetActive(player.Missing);
        }
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
		Attribute attr = GetComponent<Attribute>();
		if (attr && attr.ProcessAttack(unit, attack))
		{
			return false;
		}

		if (HPState)
		{
			HPState.Barrier.Value -= attack.Strength;
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

        //dieSubject.OnNext(gameObject);
    }

    void Revive()
    {
        coll.enabled = true;
        anim.SetTrigger("Revive");
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
}
