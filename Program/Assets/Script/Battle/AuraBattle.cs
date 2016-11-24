using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class AuraBattle : UnitBattle
{
    [SerializeField]
    protected float lastTime = Mathf.Infinity;
    [SerializeField]
    protected GameObject Effect;
    [SerializeField]
    protected float recoverTime;
    [SerializeField]
    protected ElementType element;

    float disappearStart;
    float auraStartTime;
    bool isDisappear;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        auraStartTime = Time.time;
        AuraStart();
    }

    protected virtual void AuraStart()
    {

    }

    protected virtual void AuraUpdate()
    {

    }

    void UniRxUpdate()
    {
        if (!enabled)
        {
            return;
        }

        if (isDisappear && Time.time - disappearStart > recoverTime)
        {
            isDisappear = false;
            auraStartTime = Time.time;
            AuraRecover();
        }

        if (Time.time - auraStartTime > lastTime)
        {
            DoDisappear();
        }

        AuraUpdate();
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
		bool result = IsAttackBlocked(unit, attack);

        if (IsAuraDisappear(unit, attack))
        {
            DoDisappear();
        }

		return result;
    }

    void DoDisappear()
    {
        disappearStart = Time.time;
        isDisappear = true;
        AuraDisappear();
    }

    protected virtual bool IsAuraDisappear(UnitBattle unit, Attack attack)
    {
        return false;
    }

    protected virtual bool IsAttackBlocked(UnitBattle unit, Attack attack)
    {
        return false;
    }

    protected virtual void AuraDisappear()
    {
        
    }

    protected virtual void AuraRecover()
    {

    }
}
