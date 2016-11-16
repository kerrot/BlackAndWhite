using UnityEngine;
using System.Collections;

public class UnitBattle : MonoBehaviour {

    public virtual bool Attacked(UnitBattle unit, Attack attack)
    {
        return false;
    }

    public Attack CreateAttack(AttackType type, float strength, float force)
    {
        ElementType ele = ElementType.ELEMENT_TYPE_NONE;
        Attribute attr = GetComponent<Attribute>();
        if (attr)
        {
            ele = attr.Type;
        }

        return new Attack() { Type = type, Strength = strength, Element = ele, Force = force };
    }

    public Attack CreateAttack(AttackType type, float strength)
    {
        return CreateAttack(type, strength, 0f);
    }

    public void PlaySE(AudioClip clip)
    {
        AudioSource se = GetComponent<AudioSource>();
        if (se && clip)
        {
            se.clip = clip;
            se.Play();
        }
    }
}
