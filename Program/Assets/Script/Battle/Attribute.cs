using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Attribute : MonoBehaviour {
    [SerializeField]
    private ElementType type;
    [SerializeField]
    private AuraBattle[] aura;

    public ElementType Type { get { return type; } }
	
    public bool ProcessAttack(UnitBattle unit, Attack atk)
    {
        if (IsSame(this, atk.Element))
        {
            atk.Strength /= 2;

            if (atk.Type == AttackType.ATTACK_TYPE_AURA)
            {
                return true;
            }
        }

        if (IsWeakness(this, atk.Element))
        {
            atk.Strength *= 2;
        }

        bool result = false;

        aura.ToList().ForEach(a =>
        {
            result |= a.Attacked(unit, atk);
        });

        return result;
    }

    public void SetElement(ElementType ele)
    {
        type = ele;
    }

    public static Color GetColor(ElementType ele)
    {
        switch (ele)
        {
            case ElementType.ELEMENT_TYPE_NONE:
                return new Color(1f, 1f, 1f);
            case ElementType.ELEMENT_TYPE_RED:
                return new Color(1f, 0f, 0f);
            case ElementType.ELEMENT_TYPE_GREEN:
                return new Color(0f, 1f, 0f);
            case ElementType.ELEMENT_TYPE_BLUE:
                return new Color(0f, 0f, 1f);
        }

        return new Color();
    }

    public static bool IsSame(Attribute attr, ElementType type)
    {
        if (attr)
        {
            bool result = (attr.Type == ElementType.ELEMENT_TYPE_BLUE && type == ElementType.ELEMENT_TYPE_BLUE) ||
                            (attr.Type == ElementType.ELEMENT_TYPE_RED && type == ElementType.ELEMENT_TYPE_RED) ||
                            (attr.Type == ElementType.ELEMENT_TYPE_GREEN && type == ElementType.ELEMENT_TYPE_GREEN);
            return result;
        }

        return false;
    }

    public static bool IsWeakness(Attribute attr, ElementType type)
    {
        if (attr)
        {
            bool result =   (attr.Type == ElementType.ELEMENT_TYPE_BLUE && type == ElementType.ELEMENT_TYPE_GREEN) ||
                            (attr.Type == ElementType.ELEMENT_TYPE_RED && type == ElementType.ELEMENT_TYPE_BLUE) ||
                            (attr.Type == ElementType.ELEMENT_TYPE_GREEN && type == ElementType.ELEMENT_TYPE_RED);
            return result;
        }

        return false;
    }
}
