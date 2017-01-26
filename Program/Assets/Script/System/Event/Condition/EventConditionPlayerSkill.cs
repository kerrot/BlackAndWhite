using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConditionPlayerSkill : EventCondition
{
    [SerializeField]
    private ElementType type;
    [SerializeField]
    private bool checkBase;
    [SerializeField]
    private bool checkNotBase;

    private void Start()
    {
        PlayerSkill skill = GameObject.FindObjectOfType<PlayerSkill>();
        if (skill)
        {
            skill.OnSkill.Where(t => type == ElementType.ELEMENT_TYPE_ALL || type == t).Subscribe(a =>
            {
                if (checkBase)
                {
                    if (Attribute.isBase(a))
                    {
                        completeSubject.OnNext(this);
                    }
                }
                else if (checkNotBase)
                {
                    if (!Attribute.isBase(a))
                    {
                        completeSubject.OnNext(this);
                    }
                }
                else
                {
                    completeSubject.OnNext(this);
                }
            }).AddTo(this);
        }
    }
}
