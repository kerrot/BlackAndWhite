using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class BossWood : DelaySkill 
{
	[SerializeField]
	private GameObject GreenTrap;

	protected override void DelayStart()
	{
		OnBlow.Subscribe (_ => SkillStart ());
	}

	void SkillStart()
	{
		Collider c = GetComponent<Collider> ();
		if (c) 
		{
			c.enabled = true;
			this.OnTriggerEnterAsObservable ().Subscribe (o => UniRxTriggerEnter(o));
		}

		Animator anim = GetComponent<Animator> ();
		if (anim) 
		{
			anim.enabled = true;
		}
	}

	void UniRxTriggerEnter(Collider other)
	{
		PlayerBattle player = other.gameObject.GetComponent<PlayerBattle> ();
		if (player && player.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength))) 
		{
            GameObject debuff = Instantiate(GreenTrap, player.gameObject.transform.position, Quaternion.identity) as GameObject;
            debuff.GetComponent<StopMove>().victom = player.GetComponent<UnitMove>();
        }
	}
}
