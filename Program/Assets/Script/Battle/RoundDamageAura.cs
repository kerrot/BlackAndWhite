using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class RoundDamageAura : AuraBattle
{
    [SerializeField]
    private float strength;
    [SerializeField]
    private float period;
    [SerializeField]
    private AudioClip damage;
    [SerializeField]
    private AudioClip disappearSE;

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
        Effect.SetActive(false);
        coll.enabled = false;
        AudioHelper.PlaySE(gameObject, disappearSE);
    }

    protected override void AuraRecover()
    {
        Effect.SetActive(true);
        coll.enabled = true;
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
        PlayerBattle battle = obj.GetComponent<PlayerBattle>();
        if (battle && Time.time - attackStart > period)
        {
            AudioHelper.PlaySE(gameObject, damage);

            battle.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_AURA, strength));
            attackStart = Time.time;
        }
    }
}
