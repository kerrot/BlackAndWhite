using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class KnightBattle : EnemyBattle {
    [SerializeField]
    private CorePeace peace;
    [SerializeField]
    private GameObject smoke;

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
            HPState.OnRecover.Subscribe(_ => reviveSubject.OnNext(Unit.Default)).AddTo(this);
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

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        if (!coll.enabled)
        {
            return false;
        }

        attackedSubject.OnNext(Unit.Default);

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
			//else 
			//{
			//	anim.SetTrigger("Hitted");
			//}
		}

        return false;
    }

    void Die()
    {
        coll.enabled = false;
        anim.SetTrigger("Die");

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

    void Landing()
    {
        Instantiate(smoke, transform.position, Quaternion.identity);
        coll.enabled = true;
    }
}
