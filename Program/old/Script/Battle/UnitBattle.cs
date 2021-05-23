using UnityEngine;
using System.Collections;

//all unit battle base, cannot attack or be attacked without this
public class UnitBattle : MonoBehaviour {

    public virtual bool Attacked(UnitBattle unit, Attack attack)
    {
        return false;
    }

    public Attack CreateAttack(AttackType type, float strength, float force)
    {
        return new Attack() { Type = type, Strength = strength, Element = GetElement(), Force = force };
    }

    public Attack CreateAttack(AttackType type, float strength)
    {
        return CreateAttack(type, strength, 0f);
    }

    protected virtual ElementType GetElement()
    {
        Attribute attr = GetComponent<Attribute>();
        if (attr)
        {
            return attr.Type;
        }

        return ElementType.ELEMENT_TYPE_NONE;
    }

    public void AddForce(Vector3 force)
    {
        if (force.magnitude > 0f)
        {
            Rigidbody rd = transform.GetComponent<Rigidbody>();
            if (rd)
            {
                rd.AddForce(force);
            }
        }
    }
}
