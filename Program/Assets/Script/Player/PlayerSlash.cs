﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlash : MonoBehaviour {
    public float SlashSpeed = 5;
    public GameObject TargetObject;
    public float SlashRadius = 0.5f;
    public float SlashAngle = 30;

    int slashEndHash;
	int slashingHash;
    Animator anim;
	bool continueSlash = false;

    void Awake()
    {
        anim = GetComponent<Animator>();

        slashEndHash = Animator.StringToHash("PlayerBase.SlashEnd");
		slashingHash = Animator.StringToHash("PlayerBase.Slashing");
        InputController.OnMouseSingleClick += MultiSlash;
    }
    void FixedUpdate()
    {
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		if (TargetObject != null && info.fullPathHash == slashingHash)
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

	public void SlashEnermy(GameObject enermy)
	{
		Vector3 direction = enermy.transform.position - transform.position;
		EnermyBattle battle = enermy.GetComponent<EnermyBattle> ();
		if (direction.magnitude < SlashRadius && battle.CanSlash && !anim.GetBool("IsSlashing")) {
			TargetObject = enermy;
			PlayerMove.CanRotate = false;
			anim.SetTrigger("Slash");
		}
	}

    void CheckSlash()
    {
		bool reFind = false;
        PlayerMove.CanRotate = true;
        List<GameObject> list = PlayerBattle.Enermies.GetEnermy(transform.position, SlashRadius * 2, transform.rotation * Vector3.forward, SlashAngle);
        list.ForEach(o =>
        {
			if (o == TargetObject)
			{
				reFind = true;
			}

            EnermyBattle enermy = o.GetComponent<EnermyBattle>();
            enermy.Attacked(new Attack() { Type = AttackType.ATTACK_TYPE_SLASH });
        });

		if (continueSlash) {
			continueSlash = false;
			if (reFind) {
				AutoNextSlashTartget ();
			}
		}
    }

    void MultiSlash(Vector2 mousePosition)
    {
		if (continueSlash)
		{
			AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
			if (info.fullPathHash == slashEndHash)
			{
				continueSlash = true;
				AutoNextSlashTartget ();
			}
		}
    }

	void AutoNextSlashTartget()
	{
		List<GameObject> list = PlayerBattle.Enermies.Enermies;
		foreach (GameObject o in list)
		{
			if (o != TargetObject)
			{
				SlashEnermy (o);
				if (o == TargetObject) {
					return;
				}
			}
		}
	}
}
