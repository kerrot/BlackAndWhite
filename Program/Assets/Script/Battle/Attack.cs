using UnityEngine;
using System;
using System.Collections;

[Flags]
public enum ElementType
{
    ELEMENT_TYPE_NONE = 0,
    ELEMENT_TYPE_RED = 4,
    ELEMENT_TYPE_BLUE = 1,
    ELEMENT_TYPE_GREEN = 2,
    ELEMENT_TYPE_CYAN = 3,
    ELEMENT_TYPE_YELLOW = 6,
    ELEMENT_TYPE_MAGENTA = 5,
    ELEMENT_TYPE_WHITE = 7,
    ELEMENT_TYPE_BLACK = 8,
    ELEMENT_TYPE_ALL,
};

public enum AttackType
{
    ATTACK_TYPE_NORMAL,
    ATTACK_TYPE_SLASH,
    ATTACK_TYPE_EXPLOSION,
    ATTACK_TYPE_REFLECT,
    ATTACK_TYPE_AURA,
    ATTACK_TYPE_SKILL,
    ATTACK_TYPE_ALL,
};

[Serializable]
public class Attack {
    public ElementType Element { get; set; }
    public float Strength { get; set; }
    public AttackType Type { get; set; }
    public float Force { get; set; }
}
