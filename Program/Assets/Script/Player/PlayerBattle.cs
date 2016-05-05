using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBattle : MonoBehaviour {
    public EnemyGenerator enermies;
    public float AttackAngle = 60;

    Animator anim;
    float AttackRadius;
    float SlashRadius;

    void Start()
    {
        enermies.OnEnermyClicked += AttackEnermy;
        anim = GetComponent<Animator>();
        AttackRadius = transform.FindChild("AttackRegion").transform.localScale.x / 2;
        SlashRadius = transform.FindChild("SlashRegion").transform.localScale.x / 2;
    }

    void AttackEnermy (GameObject enermy)
    {
        EnermyBattle battle = enermy.GetComponent<EnermyBattle>();
        Vector3 direction = enermy.transform.position - transform.position;
        
        if (direction.magnitude < AttackRadius && !battle.CanSlash)
        {
            anim.SetTrigger("Attack");
        }
        else if(direction.magnitude < SlashRadius && battle.CanSlash)
        {
            anim.SetTrigger("Slash");
        }
    }

    void AttackHit()
    {
        List<GameObject> list = enermies.GetEnermy(transform.position, AttackRadius, transform.rotation * Vector3.forward, AttackAngle);
        list.ForEach(o =>
        {
            EnermyBattle enermy = o.GetComponent<EnermyBattle>();
            enermy.Attacked(new AttackBase());
        });
    }
}
