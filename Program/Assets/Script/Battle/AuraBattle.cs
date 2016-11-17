using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class AuraBattle : UnitBattle
{
    [SerializeField]
    protected GameObject Effect;
    [SerializeField]
    protected float recoverTime;

    bool isUpdate = false;
    float disappearStart;
    bool isDisappear;

    void StartUpdate()
    {
        if (!isUpdate)
        {
            this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
            isUpdate = true;
        }
    }

    void UniRxUpdate()
    {
        if (isDisappear && Time.time - disappearStart > recoverTime)
        {
            isDisappear = false;
            AuraRecover();
        }
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
		bool result = IsAttackBlocked(unit, attack);

        if (IsAuraDisappear(unit, attack))
        {
            disappearStart = Time.time;
            isDisappear = true;
            AuraDisappear();
            StartUpdate();
        }

		return result;
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
