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
        if (IsPure(atk.Element))
        {
            atk.Strength = PureProcess(atk.Element, atk.Strength);
        }
        else
        {
            float strength = atk.Strength / 2;
            float tmp = 0;
            if ((atk.Element & ElementType.ELEMENT_TYPE_RED) != 0)
            {
                tmp += PureProcess(ElementType.ELEMENT_TYPE_RED, strength);
            }
            if ((atk.Element & ElementType.ELEMENT_TYPE_GREEN) != 0)
            {
                tmp += PureProcess(ElementType.ELEMENT_TYPE_GREEN, strength);
            }
            if ((atk.Element & ElementType.ELEMENT_TYPE_BLUE) != 0)
            {
                tmp += PureProcess(ElementType.ELEMENT_TYPE_BLUE, strength);
            }
            atk.Strength = tmp;
        }

        bool result = false;
        if (atk.Type == AttackType.ATTACK_TYPE_AURA && IsResist(type, atk.Element))
        {
            result = true;
        }

        aura.ToList().ForEach(a =>
        {
            result |= a.Attacked(unit, atk);
        });

        return result;
    }

    float PureProcess(ElementType ele, float strength)
    {
        if (IsResist(type, ele))
        {
            return strength / 2;
        }
        else if (IsWeakness(type, ele))
        {
            return strength * 2;
        }
        else if (IsWeakness(ele, type))
        {
            return 0f;
        }

        return strength;
    }

    public void SetElement(bool active, ElementType ele)
    {
        if (isBase(ele))
        {
            if (active)
            {
                type |= ele;
            }
            else
            {
                type &= ~ele;
            }
        }
    }

	public static Color GetColor(ElementType ele, float alpha)
    {
        if (ele == ElementType.ELEMENT_TYPE_NONE)
        {
			return new Color(0.3f, 0.3f, 0.3f, alpha);
        }

		Color c = new Color(0f, 0f, 0f, alpha);
        if ((ele & ElementType.ELEMENT_TYPE_RED) != 0)
        {
            c.r = 1f;
        }
        if ((ele & ElementType.ELEMENT_TYPE_GREEN) != 0)
        {
            c.g = 1f;
        }
        if ((ele & ElementType.ELEMENT_TYPE_BLUE) != 0)
        {
            c.b = 1f;
        }


        return c;
    }

    public static bool isBase(ElementType ele)
    {
        bool result = ele == ElementType.ELEMENT_TYPE_RED ||
                      ele == ElementType.ELEMENT_TYPE_GREEN ||
                      ele == ElementType.ELEMENT_TYPE_BLUE;
        return result;
    }

    public static bool IsResist(ElementType victom, ElementType attacker)
    {
        bool result = (victom == ElementType.ELEMENT_TYPE_BLUE && attacker == ElementType.ELEMENT_TYPE_BLUE) ||
                      (victom == ElementType.ELEMENT_TYPE_RED && attacker == ElementType.ELEMENT_TYPE_RED) ||
                      (victom == ElementType.ELEMENT_TYPE_GREEN && attacker == ElementType.ELEMENT_TYPE_GREEN) ||
                      victom == ElementType.ELEMENT_TYPE_BLACK && attacker != ElementType.ELEMENT_TYPE_WHITE;
        return result;
    }

    public static bool IsPure(ElementType ele)
    {
        return ele == ElementType.ELEMENT_TYPE_NONE ||
                ele == ElementType.ELEMENT_TYPE_RED ||
                ele == ElementType.ELEMENT_TYPE_GREEN ||
                ele == ElementType.ELEMENT_TYPE_BLUE ||
                ele == ElementType.ELEMENT_TYPE_BLACK;
    }

    public static bool IsWeakness(ElementType victom, ElementType attacker)
    {
        bool result = false;
        result |= victom == ElementType.ELEMENT_TYPE_RED && ((attacker & ElementType.ELEMENT_TYPE_BLUE) != 0);
        result |= victom == ElementType.ELEMENT_TYPE_GREEN && ((attacker & ElementType.ELEMENT_TYPE_RED) != 0);
        result |= victom == ElementType.ELEMENT_TYPE_BLUE && ((attacker & ElementType.ELEMENT_TYPE_GREEN) != 0);
        result |= victom == ElementType.ELEMENT_TYPE_BLACK && attacker == ElementType.ELEMENT_TYPE_WHITE;

        return result;
    }
}
