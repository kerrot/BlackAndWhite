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
        if (Attribute.IsWeakness(attr.Type, attack.Element) && attack.Type == AttackType.ATTACK_TYPE_EXPLOSION)
        {
            if (HPState)
            {
                HPState.Barrier.Value -= attack.Strength;
                if (HPState.Barrier.Value <= 0)
                {
                    Die();
                }
            }
        }
        else
        {
            unit.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_REFLECT, 0f));
        }

        return false;
    }

    void Die()
    {
        coll.enabled = false;
        anim.SetTrigger("Die");

        dieSubject.OnNext(gameObject);
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
