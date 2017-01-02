using UnityEngine;
using System.Collections;
using System.Linq;

public class DeadAction : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem flash;
    [SerializeField]
    private float duration;

    public Attack Atk;
    public UnitBattle Attacker;

    void Start()
	{
        ParticleSystem[] pars = GetComponentsInChildren<ParticleSystem>();
        pars.ToList().ForEach(p =>
        {
            ParticleSystem.MainModule mod = p.main;
            mod.startColor = Attribute.GetColor(Atk.Element, 1f);
        });

        ParticleSystem.MainModule flashmod = flash.main;
        Color tmpC = flashmod.startColor.color;
        tmpC += Color.gray;
        flashmod.startColor = tmpC;

        Destroy(gameObject, duration);

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