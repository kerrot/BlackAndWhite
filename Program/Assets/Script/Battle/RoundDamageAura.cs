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
    private AudioClip disappear;

    float attackStart;

    Collider coll;

    void Start()
    {
        coll = GetComponent<Collider>();
    }

    protected override bool IsAuraDisappear(UnitBattle unit, Attack attack)
    {
        return attack.Element == ElementType.ELEMENT_TYPE_BLUE;
    }

    protected override void AuraDisappear()
    {
        Effect.SetActive(false);
        coll.enabled = false;
        AudioHelper.PlaySE(gameObject, disappear);
    }

    protected override void AuraRecover()
    {
        Effect.SetActive(true);
        coll.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Attack(other.gameObject);
    }

    void OnTriggerStay(Collider other)
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
