using UnityEngine;
using System.Collections;

public class UnitBattle : MonoBehaviour {

    public virtual bool Attacked(UnitBattle unit, Attack attack)
    {
        return false;
    }

    public Attack CreateAttack(AttackType type, float strength)
    {
        ElementType ele = ElementType.ELEMENT_TYPE_NONE;
        Attribute attr = GetComponent<Attribute>();
        if (attr)
        {
            ele = attr.Type;
        }

        return new Attack() { Type = type, Strength = strength, Element = ele };
    }
}
