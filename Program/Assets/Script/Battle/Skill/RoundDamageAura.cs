using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

// damage all nearby enemy
public class RoundDamageAura : AuraBattle
{
    [SerializeField]
    private float strength;
    [SerializeField]
    private float period;
    [SerializeField]
    private GameObject burn;
    [SerializeField]
    private AudioClip disappearSE;

    // damage event
    private Subject<Unit> damageSubject = new Subject<Unit>();
    public IObservable<Unit> OnDamage { get { return damageSubject; } }

    float attackStart;

    Collider coll;

    protected override void AuraStart()
    {
        coll = GetComponent<Collider>();
        this.OnTriggerEnterAsObservable().Subscribe(o => UniRxOnTriggerEnter(o));
        this.OnTriggerStayAsObservable().Subscribe(o => UniRxOnTriggerStay(o));
    }

    protected override bool IsAuraDisappear(UnitBattle unit, Attack attack)
    {
        return Attribute.IsWeakness(element, attack.Element);
    }

    protected override void AuraDisappear()
    {
        if (Effect)
        {
            Effect.SetActive(false);
        }
        else
        {
            Debug.Log(gameObject + "Effect Missing");
        }

        if (coll)
        {
            coll.enabled = false;
        }
        else
        {
            Debug.Log(gameObject + "Collider Missing");
        }

        
        AudioHelper.PlaySE(gameObject, disappearSE);
    }

    protected override void AuraRecover()
    {
        if (Effect)
        {
            Effect.SetActive(true);
        }
        else
        {
            Debug.Log(gameObject + "Effect Missing");
        }

        if (coll)
        {
            coll.enabled = true;
        }
        else
        {
            Debug.Log(gameObject + "Collider Missing");
        }
    }

    void UniRxOnTriggerEnter(Collider other)
    {
        Attack(other.gameObject);
    }

    void UniRxOnTriggerStay(Collider other)
    {
        Attack(other.gameObject);
    }

    void Attack(GameObject obj)
    {
        // attack enemy every [period] seccond
        PlayerBattle battle = obj.GetComponent<PlayerBattle>();
        if (battle && Time.time - attackStart > period)
        {
            if (battle.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_AURA, strength)))
            {
                GameObject b = Instantiate(burn, battle.transform.position, Quaternion.identity);
                b.transform.parent = battle.transform;

                attackStart = Time.time;

                damageSubject.OnNext(Unit.Default);
            }
        }
    }
}
