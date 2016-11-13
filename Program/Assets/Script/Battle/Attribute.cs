using UnityEngine;
using System.Collections;

public class Attribute : MonoBehaviour {
    [SerializeField]
    private ElementType type;

    public ElementType Type { get { return type; } }
	
    public bool ProcessAttack(Attack atk)
    {
        if (atk.Element == type)
        {
            atk.Strength /= 2;

            if (atk.Type == AttackType.ATTACK_TYPE_AURA)
            {
                return true;
            }
        }

        if ((atk.Element == ElementType.ELEMENT_TYPE_BLUE && type == ElementType.ELEMENT_TYPE_RED) ||
            (atk.Element == ElementType.ELEMENT_TYPE_BLUE && type == ElementType.ELEMENT_TYPE_RED) ||
            (atk.Element == ElementType.ELEMENT_TYPE_BLUE && type == ElementType.ELEMENT_TYPE_RED) )
        {
            atk.Strength *= 2;
        }

        return false;
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
}
