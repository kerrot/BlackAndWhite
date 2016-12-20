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

    public bool IsAura { get { return isRecover; } }

    float disappearStart;
    float auraStartTime;
    bool isDisappear;
    bool isRecover;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        auraStartTime = Time.time;
        AuraStart();
    }

    protected override ElementType GetElement()
    {
        return element;
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
            DoRecover();
        }

        if (isRecover && Time.time - auraStartTime > lastTime)
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

    protected void DoRecover()
    {
        disappearStart = Time.time;
        isDisappear = false;
        isRecover = true;
        auraStartTime = Time.time;
        AuraRecover();
    }

    protected void DoDisappear()
    {
        disappearStart = Time.time;
        isDisappear = true;
        isRecover = false;
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
