using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBattle : MonoBehaviour {
    static public EnemyGenerator Enermies;
    public GameObject AttackRegion;
    public GameObject SlashRegion;
    public float AttackAngle = 60;

    Animator anim;
    float AttackRadius = 1.3f;
    float SlashRadius = 3f;

    void Start()
    {
        Enermies = GetComponent<EnemyGenerator>();
        Enermies.OnEnermyClicked += AttackEnermy;
        anim = GetComponent<Animator>();
        AttackRadius = AttackRegion.transform.localScale.x / 2;
        SlashRadius = SlashRegion.transform.localScale.x / 2;

        SlashingAnim.PlayerCollider = GetComponent<Collider>();
    }

    void AttackEnermy (GameObject enermy)
    {
        EnermyBattle battle = enermy.GetComponent<EnermyBattle>();
        Vector3 direction = enermy.transform.position - transform.position;
        
        if (direction.magnitude < AttackRadius && !battle.CanSlash)
        {
            PlayerMove.CanRotate = false;
            anim.SetTrigger("Attack");
        }
        else if(direction.magnitude < SlashRadius && battle.CanSlash)
        {
            GetComponent<PlayerSlash>().TargetObject = enermy;
            PlayerMove.CanRotate = false;
            anim.SetTrigger("Slash");
        }
    }

    void AttackHit()
    {
        List<GameObject> list = Enermies.GetEnermy(transform.position, AttackRadius, transform.rotation * Vector3.forward, AttackAngle);
        list.ForEach(o =>
        {
            EnermyBattle enermy = o.GetComponent<EnermyBattle>();
            enermy.Attacked(new AttackBase());
        });
    }
}
