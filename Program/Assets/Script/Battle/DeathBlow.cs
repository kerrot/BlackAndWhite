using UnityEngine;
using System.Collections;
using System.Linq;

public class DeathBlow : UnitBattle
{
    [SerializeField]
    private float strength;
    [SerializeField]
    private float force;

    void Start()
	{
        ParticleSystem par = GetComponent<ParticleSystem>();
        par.startColor = Attribute.GetColor(GetComponent<Attribute>().Type, 1.0f);

        Destroy(gameObject, par.duration);

        Collider[] cs = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        cs.ToList().ForEach(c =>
        {
            EnemyBattle battle = c.gameObject.GetComponent<EnemyBattle>();
            if (battle)
            {
                battle.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_EXPLOSION, strength, force));
            }
        });
    }
}