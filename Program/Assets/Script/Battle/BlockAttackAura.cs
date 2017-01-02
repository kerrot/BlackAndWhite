using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class BlockAttackAura : AuraBattle
{
    [SerializeField]
    private float blockValue;
    [SerializeField]
    private float emissionRate;
    [SerializeField]
    private AudioClip blockSE;

    private Subject<Unit> blockSubject = new Subject<Unit>();
    public IObservable<Unit> OnBlock { get { return blockSubject; } }

    float nowBlock;
	ParticleSystem.EmissionModule em;

    protected override void AuraStart()
    {
		nowBlock = blockValue;
        if (Effect)
        {
            em = Effect.GetComponent<ParticleSystem>().emission;
        }
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
                var rate = em.rateOverTime;
                rate.constantMax = nowBlock * emissionRate;
                em.rateOverTime = rate;
            }
            else
            {
                em.enabled = false;
            }
        }

        if (result)
        {
            blockSubject.OnNext(Unit.Default);
        }

        return result;
	}

    protected override void AuraDisappear()
    {
        if (GetComponent<ParticleSystem>())
        {
            em.enabled = false;
        }
        else
        {
            Debug.Log(gameObject + "ParticleSystem Missing");
        }
    }

    protected override void AuraRecover()
    {
		nowBlock = blockValue;
		em.enabled = true;
		var rate = em.rateOverTime;
		rate.constantMax = nowBlock * emissionRate;
		em.rateOverTime = rate;	
    }
}
