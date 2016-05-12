using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlash : MonoBehaviour {
    public float SlashSpeed = 5;
    public GameObject TargetObject;
    public float SlashRadius = 0.5f;
    public float SlashAngle = 30;

    int slashEndHash;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        slashEndHash = Animator.StringToHash("PlayerBase.SlashEnd");
        InputController.OnMouseSingleClick += MultiSlash;
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
        PlayerMove.CanRotate = true;
        List<GameObject> list = PlayerBattle.Enermies.GetEnermy(transform.position, SlashRadius * 2, transform.rotation * Vector3.forward, SlashAngle);
        list.ForEach(o =>
        {
            EnermyBattle enermy = o.GetComponent<EnermyBattle>();
            enermy.Attacked(new Attack() { Type = AttackType.ATTACK_TYPE_SLASH });
        });
    }

    void MultiSlash(Vector2 mousePosition)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash == slashEndHash)
        {
            List<GameObject> list = PlayerBattle.Enermies.Enermies;
            foreach (GameObject o in list)
            {
                if (o != TargetObject)
                {
                    EnermyBattle battle = o.GetComponent<EnermyBattle>();
                    if (battle != null && battle.CanSlash && (o.transform.position - transform.position).magnitude < PlayerBattle.SlashRadius)
                    {
                        TargetObject = o;
                        PlayerMove.CanRotate = false;
                        anim.SetTrigger("Slash");
                        return;
                    }
                }
            }
        }
    }
}
