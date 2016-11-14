using UnityEngine;
using System;
using System.Collections;

public enum ElementType
{
    ELEMENT_TYPE_NONE,
    ELEMENT_TYPE_RED,
    ELEMENT_TYPE_BLUE,
    ELEMENT_TYPE_GREEN,
    ELEMENT_TYPE_WHITE,
    ELEMENT_TYPE_BLACK,
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
