﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttribute : Attribute
{
    
    [SerializeField]
    private MeshRenderer Lance;
    [SerializeField]
    private GameObject LanceEffect;

    private BoolReactiveProperty redOn = new BoolReactiveProperty();
    private BoolReactiveProperty greenOn = new BoolReactiveProperty();
    private BoolReactiveProperty blueOn = new BoolReactiveProperty();
    private Dictionary<ElementType, BoolReactiveProperty> mapping = new Dictionary<ElementType, BoolReactiveProperty>();

    public IObservable<bool> RedOn { get { return redOn; } }
    public IObservable<bool> GreenOn { get { return greenOn; } }
    public IObservable<bool> BlueOn { get { return blueOn; } }

    Material lanceEffectmat;
    

    void Awake()
    {
        lanceEffectmat = LanceEffect.GetComponentInChildren<MeshRenderer>().material;
        mapping.Add(ElementType.ELEMENT_TYPE_RED, redOn);
        mapping.Add(ElementType.ELEMENT_TYPE_GREEN, greenOn);
        mapping.Add(ElementType.ELEMENT_TYPE_BLUE, blueOn);
    }

    public void AttributeChange(bool active, ElementType ele)
    {
        SetElement(active, ele);

        Color color = Attribute.GetColor(type, 0.5f) * 2;
        Lance.material.SetColor("_EmissionColor", color);
        lanceEffectmat.SetColor("_EmissionColor", color);

        LanceEffect.gameObject.SetActive(type != ElementType.ELEMENT_TYPE_NONE);

        if (mapping.ContainsKey(ele))
        {
            mapping[ele].Value = active;
        }
    }
}