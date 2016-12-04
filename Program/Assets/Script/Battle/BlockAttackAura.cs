﻿using UnityEngine;
using System.Collections;

public class BlockAttackAura : AuraBattle
{
    [SerializeField]
    private float blockValue;
    [SerializeField]
    private float emissionRate;
    [SerializeField]
    private AudioClip blockSE;

    float nowBlock;
	ParticleSystem.EmissionModule em;

    protected override void AuraStart()
    {
		nowBlock = blockValue;
		em = Effect.GetComponent<ParticleSystem> ().emission;
    }

    protected override bool IsAuraDisappear(UnitBattle unit, Attack attack)
    {
		return nowBlock <= 0;
    }

	protected override bool IsAttackBlocked(UnitBattle unit, Attack attack)
	{
		bool result = nowBlock > 0;
        if (result)
        {
            AudioHelper.PlaySE(gameObject, blockSE);
            bool isWeak = Attribute.IsWeakness(element, attack.Element);

            if (attack.Type == AttackType.ATTACK_TYPE_EXPLOSION || isWeak)
            {
                nowBlock = 0;
                result = !isWeak;
            }
            else
            {
                nowBlock -= attack.Strength;
                if (nowBlock < 0)
                {
                    nowBlock = 0;
                }
            }

            if (nowBlock > 0)
            {
                var rate = em.rate;
                rate.constantMax = nowBlock * emissionRate;
                em.rate = rate;
            }
            else
            {
                em.enabled = false;
            }
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
		rate.constantMax = nowBlock * emissionRate;
		em.rate = rate;	
    }
}
