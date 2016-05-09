using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlash : MonoBehaviour {
    public float SlashSpeed = 5;
    public GameObject TargetObject;
    public float SlashRadius = 0.5f;
    public float SlashAngle = 30;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (TargetObject != null)
        {
            transform.LookAt(TargetObject.transform);

            Vector3 direction = transform.forward;
            direction.y = 0;
            transform.position += direction * SlashSpeed * Time.deltaTime;

            if ((TargetObject.transform.position - transform.position).magnitude < SlashRadius)
            {
                anim.SetTrigger("SlashEnd");
                TargetObject = null;
            }
        }
    }

    void CheckSlash()
    {
        List<GameObject> list = PlayerBattle.Enermies.GetEnermy(transform.position, SlashRadius * 2, transform.rotation * Vector3.forward, SlashAngle);
        list.ForEach(o =>
        {
            EnermyBattle enermy = o.GetComponent<EnermyBattle>();
            enermy.Attacked(new AttackBase() { Type = AttackType.ATTACK_TYPE_SLASH });
        });
    }
}
