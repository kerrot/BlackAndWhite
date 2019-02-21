using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// make the attack invalid
public class ImmunityAura : AuraBattle {

    [SerializeField]
    private List<ImmunityType> data = new List<ImmunityType>();
    [SerializeField]
    private bool reflict;   //reflict attack to attacker
    [SerializeField]
    private AudioClip blockSE;
    [SerializeField]
    private float strength;
    [SerializeField]
    private GameObject UIPosition;  // ui display
    [SerializeField]
    private GameObject ImmUI;

    // block event
    private Subject<Unit> blockSubject = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnBlock { get { return blockSubject; } }

    [Serializable]
    public struct ImmunityType
    {
        public AttackType type;
        public ElementType element;
    }

    private GameObject imm; // the instance of ImmUI

    protected override void AuraStart()
    {
        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            imm = ui.CreateUI(ImmUI);
            this.OnDestroyAsObservable().Subscribe(_ => DestroyObject(imm));
        }
    }

    protected override void AuraUpdate()
    {
        // follow the owner corresponding to screen
        if (imm)
        {
            imm.transform.position = Camera.main.WorldToScreenPoint(UIPosition.transform.position);
        }
    }

    protected override bool IsAttackBlocked(UnitBattle unit, Attack attack)
    {
		if (Effect && !Effect.activeSelf)
        {
            return false;
        }

        bool result = false;
        foreach (ImmunityType d in data)
        {
            result |= (d.type == AttackType.ATTACK_TYPE_ALL && d.element == attack.Element) ||  //block all attack with certain element type
                    (d.element == ElementType.ELEMENT_TYPE_ALL && d.type == attack.Type) ||  //block all element with certain attack type
                    (d.type == attack.Type && d.element == attack.Element);     //block correspondence element and attack type

            if (result)
            {
                if (reflict && attack.Type != AttackType.ATTACK_TYPE_REFLECT)
                {
                    unit.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_REFLECT, strength));
                }

                AudioHelper.PlaySE(gameObject, blockSE);

                break;
            }
        }

        if (imm)
        {
            imm.SetActive(false);
        }
        
        if (result)
        {
            if (imm)
            {
                imm.SetActive(true);
            }
            blockSubject.OnNext(Unit.Default);
        }

        return result;
    }

    protected override bool IsAuraDisappear(UnitBattle unit, Attack attack)
    {
        return Attribute.IsWeakness(element, attack.Element);
    }

    protected override void AuraDisappear()
    {
		if (Effect) 
		{
			Effect.SetActive(false);
		}
    }

    protected override void AuraRecover()
    {
		if (Effect) 
		{
			Effect.SetActive(true);
		}
    }
}
