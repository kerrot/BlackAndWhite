using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class BossWater : DelaySkill 
{
	protected override void DelayStart()
	{
		OnBlow.Subscribe (_ => SkillStart ()).AddTo(this);
	}

	void SkillStart()
	{
		Collider c = GetComponent<Collider> ();
		if (c) 
		{
			c.enabled = true;
			this.OnTriggerEnterAsObservable ().Subscribe (o => UniRxTriggerEnter(o));
		}
	}

	void UniRxTriggerEnter(Collider other)
	{
		PlayerBattle player = other.gameObject.GetComponent<PlayerBattle> ();
		if (player) 
		{
			player.Attacked (this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength, force));
		}
	}
}
