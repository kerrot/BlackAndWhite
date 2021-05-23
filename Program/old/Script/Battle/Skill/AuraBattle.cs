using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

//Aura base class, all aura derive this class
public class AuraBattle : UnitBattle
{
    [SerializeField]
    protected float lastTime = Mathf.Infinity;  // aura last time
    [SerializeField]
    protected GameObject Effect;
    [SerializeField]
    protected float recoverTime;    //effect restart period
    [SerializeField]
    protected ElementType element;  // all aura have element type

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

    //for derive class start
    protected virtual void AuraStart()
    {

    }

    //for derive class update
    protected virtual void AuraUpdate()
    {

    }

    // check if aura last time
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

    // when owner being attacked
    public override bool Attacked(UnitBattle unit, Attack attack)
    {
		bool result = IsAttackBlocked(unit, attack);

        // aura is possible to disappear by attack
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
