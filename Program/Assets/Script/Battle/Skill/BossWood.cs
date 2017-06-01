using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

//boss skill (only attack player)
public class BossWood : DelaySkill 
{
	[SerializeField]
	private GameObject GreenTrap; //Trapped effect

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
			this.OnTriggerStayAsObservable ().Subscribe (o => UniRxTriggerStay(o));
		}

		Animator anim = GetComponent<Animator> ();
		if (anim) 
		{
			anim.enabled = true;
		}
	}

    // stop player movement
	void UniRxTriggerStay(Collider other)
	{
		PlayerBattle player = other.gameObject.GetComponent<PlayerBattle> ();
		if (player && player.GetComponent<UnitMove>().CanMove 
                    && player.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength))) 
		{
            GameObject debuff = Instantiate(GreenTrap, player.gameObject.transform.position, Quaternion.identity) as GameObject;
            debuff.GetComponent<StopMove>().victom = player.GetComponent<UnitMove>();
            // Follow victom
            debuff.transform.parent = player.transform.parent;
        }
	}
}
