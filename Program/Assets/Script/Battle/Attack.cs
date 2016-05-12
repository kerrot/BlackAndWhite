using UnityEngine;
using System.Collections;

public enum ElementType
{
    ELEMENT_TYPE_NONE,
    ELEMENT_TYPE_RED,
    ELEMENT_TYPE_BLUE,
    ELEMENT_TYPE_GREEN,
    ELEMENT_TYPE_WHITE,
    ELEMENT_TYPE_BLACK,
};

public enum AttackType
{
    ATTACK_TYPE_NORMAL,
    ATTACK_TYPE_SLASH,
};

public class Attack {
    public ElementType Element { get; set; }
    public int Strength { get; set; }
    public AttackType Type { get; set; }
}
