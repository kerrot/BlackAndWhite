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
    private UnitType unit = UnitType.ENEMY;
    [SerializeField]
    private AttackType type = AttackType.ATTACK_TYPE_NORMAL;

    public enum UnitType
    {
        ALL,
        ENEMY,
        PLAYER,
    }

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
                if (unit == UnitType.ALL || (unit == UnitType.ENEMY && (battle is EnemyBattle || battle is KnightBattle))
                                         || (unit == UnitType.PLAYER && battle is PlayerBattle))
                {
                    battle.Attacked(this, CreateAttack(type, strength, force));
                }
            }
        });
    }
}