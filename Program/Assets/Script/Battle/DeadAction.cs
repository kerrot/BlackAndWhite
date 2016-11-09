using UnityEngine;
using System.Collections;
using System.Linq;

public class DeadAction : MonoBehaviour
{
    public Attack A = new Attack() { Type = AttackType.ATTACK_TYPE_EXPLOSION };
    public UnitBattle Attacker;

    void Start()
	{
		Destroy(gameObject, GetComponent<ParticleSystem>().duration);


        Collider[] cs = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        cs.ToList().ForEach(c =>
        {
            EnemyBattle battle = c.gameObject.GetComponent<EnemyBattle>();
            if (battle != null)
            {
                battle.Attacked(Attacker, A);
            }
        });
    }
}