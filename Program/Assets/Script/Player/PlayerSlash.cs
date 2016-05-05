using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlash : MonoBehaviour {
    public float slashSpeed = 5;
    public GameObject TargetObject;

    public float angle;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (anim.GetBool("IsSlashing"))
        {
            angle = Vector3.Angle(transform.forward, TargetObject.transform.position - transform.position);
            if (angle > 90)
            {
                anim.SetTrigger("SlashEnd");
                anim.SetBool("IsSlashing", false);
            }

            Vector3 direction = transform.forward;
            direction.y = 0;
            transform.position += direction * slashSpeed * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CheckSlash(collision.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        CheckSlash(collision.gameObject);
    }

    void CheckSlash(GameObject obj)
    {
        if (anim.GetBool("IsSlashing"))
        {
            EnermyBattle enermy = obj.GetComponent<EnermyBattle>();
            if (enermy != null && enermy.CanSlash)
            {
                enermy.Attacked(new AttackBase() { Type = AttackType.ATTACK_TYPE_SLASH });
            }
        }
    }
}
