using UnityEngine;
using System.Collections;

public class BlockAttackAura : AuraBattle
{
    [SerializeField]
    private float blockValue;
    [SerializeField]
    private AudioClip block;

    float nowBlock;
	ParticleSystem.EmissionModule em;

    void Start()
    {
		nowBlock = blockValue;
		em = GetComponent<ParticleSystem> ().emission;
    }

    protected override bool IsAuraDisappear(UnitBattle unit, Attack attack)
    {
		return nowBlock <= 0;
    }

	protected override bool IsAttackBlocked(UnitBattle unit, Attack attack)
	{
		bool result = nowBlock > 0;

		AudioSource au;

		if (attack.Type == AttackType.ATTACK_TYPE_EXPLOSION) {
			nowBlock = 0;
		} 
		else {
			nowBlock -= attack.Strength;
		}

		if (nowBlock > 0) {
			var rate = em.rate;
			rate.constantMax = nowBlock;
			em.rate = rate;	
		} 
		else {
			em.enabled = false;
		}

		return result;
	}

    protected override void AuraDisappear()
    {
		em.enabled = false;
    }

    protected override void AuraRecover()
    {
		nowBlock = blockValue;
		em.enabled = true;
		var rate = em.rate;
		rate.constantMax = nowBlock;
		em.rate = rate;	
    }
}
