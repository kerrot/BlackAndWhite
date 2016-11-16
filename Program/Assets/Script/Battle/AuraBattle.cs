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
        if (Time.time - disappearStart > recoverTime)
        {
            AuraRecover();
        }
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
		bool result = IsAttackBlocked(unit, attack);

        if (IsAuraDisappear(unit, attack))
        {
            disappearStart = Time.time;
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
