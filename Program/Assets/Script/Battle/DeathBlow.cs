using UnityEngine;
using System.Collections;
using System.Linq;

public class DeathBlow : UnitBattle
{
    [SerializeField]
    private float strength;
    [SerializeField]
    private float force;
    [SerializeField]
    private ParticleSystem part;
    [SerializeField]
    private bool all;

    void Start()
	{
        if (!part)
        {
            part = GetComponent<ParticleSystem>();
        }

        ParticleSystem.MainModule mod = part.main;
        mod.startColor = Attribute.GetColor(GetComponent<Attribute>().Type, 1.0f);

        Destroy(gameObject, mod.duration);

        Collider[] cs = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        cs.ToList().ForEach(c =>
        {
            UnitBattle battle = c.gameObject.GetComponent<UnitBattle>();
            if (battle)
            {
                if (all || battle is EnemyBattle)
                {
                    if (strength > 0)
                    {
                        battle.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_EXPLOSION, strength, force));
                    }
                    else
                    {
                        battle.AddForce((battle.transform.position - transform.position).normalized * force);
                    }
                }
            }
        });
    }
}