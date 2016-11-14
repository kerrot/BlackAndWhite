﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Immunity : MonoBehaviour {
    [SerializeField]
    private GameObject display;
    [SerializeField]
    private List<ImmunityType> data = new List<ImmunityType>();
    [SerializeField]
    private bool reflict;

    [Serializable]
    public struct ImmunityType
    {
        public AttackType type;
        public ElementType element;
    }

    void OnEnable()
    {
        if (display)
        {
            display.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (display)
        {
            display.SetActive(false);
        }
    }

    public bool CheckImmunity(UnitBattle unit, Attack attack)
    {
        bool result = false;
        foreach (ImmunityType d in data)
        {
            result = (d.type == AttackType.ATTACK_TYPE_ALL && d.element == attack.Element) ||
                    (d.element == ElementType.ELEMENT_TYPE_ALL && d.type == attack.Type) ||
                    (d.type == attack.Type && d.element == attack.Element);

            if (result)
            {
				if (reflict && attack.Type != AttackType.ATTACK_TYPE_REFLECT)
                {
                    UnitBattle battle = GetComponent<UnitBattle>();
                    if (battle)
                    {
                        unit.Attacked(GetComponent<UnitBattle>(), battle.CreateAttack(AttackType.ATTACK_TYPE_REFLECT, 0f));
                    }
                }

                if (display)
                {
                    AudioSource au = display.GetComponent<AudioSource>();
                    if (au)
                    {
                        au.Play();
                    }
                }

                break;
            }
        }

        return result;
    }
}