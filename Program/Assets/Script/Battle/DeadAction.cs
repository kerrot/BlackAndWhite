using UnityEngine;
using System.Collections;
using System.Linq;

public class DeadAction : MonoBehaviour
{
    public Attack Atk;
    public UnitBattle Attacker;

    void Start()
	{
        ParticleSystem par = GetComponent<ParticleSystem>();
        par.startColor = Attribute.GetColor(Atk.Element, 0.3f);

        Destroy(gameObject, par.duration);


        Collider[] cs = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        cs.ToList().ForEach(c =>
        {
            EnemyBattle battle = c.gameObject.GetComponent<EnemyBattle>();
            if (battle != null && battle != Attacker)
            {
                battle.Attacked(Attacker, Atk);
            }
        });
    }
}