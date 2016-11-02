﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyBattle : MonoBehaviour {
    [SerializeField]
    private float HPMax;
    [SerializeField]
    private float barrierStrength;
    [SerializeField]
    private float recoverTime;
    [SerializeField]
    private float showHPTime;
    [SerializeField]
    private Transform HPUICenter;

    public UnitAction OnDie;
    
    public GameObject DeadAction;
    public Vector3 DeadEffectOffset;

    EnemySlash slash;

    HPBarUI hpUI;
    float showHPStart;

    Animator anim;
    
    float currentBarrier;
    float currentHP;
    float currentRecover;

    //Start change to Awake, because Instantiate not call Start but Awake
    void Awake()
    {
        slash = GetComponent<EnemySlash>();
        anim = GetComponent<Animator>();
        currentBarrier = barrierStrength;
        currentHP = HPMax;
        currentRecover = 0;
    }

    void Start()
    {
        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        { 
            GameObject tmp = ui.CreateHPUI();
            hpUI = tmp.GetComponent<HPBarUI>();
            hpUI.SetHPMax(HPMax);
            hpUI.SetHPCurrent(currentHP);
            hpUI.SetBarrierMax(barrierStrength);
            hpUI.SetBarrierCurrent(currentBarrier);
            hpUI.SetRecoverMax(recoverTime);
            hpUI.SetRecoverCurrent(currentRecover);
            hpUI.SetRecoverEnable(false);
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());       
    }

    void UniRxUpdate()
    {
        if (hpUI.gameObject.activeSelf)
        {
            hpUI.transform.position = Camera.main.WorldToScreenPoint(HPUICenter.transform.position);

            if (currentBarrier <= 0)
            {
                hpUI.SetRecoverEnable(true);
                currentRecover += Time.deltaTime;
                if (currentRecover > recoverTime)
                {
                    currentRecover = 0;
                    showHPStart = Time.time;
                    hpUI.SetRecoverEnable(false);
                    currentBarrier = barrierStrength;
                }
            }

            if (currentRecover == 0 && Time.time - showHPStart > showHPTime)
            {
                hpUI.gameObject.SetActive(false);
            }

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        hpUI.SetHPCurrent(currentHP);
        hpUI.SetBarrierCurrent(currentBarrier);
        hpUI.SetRecoverCurrent(currentRecover);
    }

    public bool Attacked(Attack attack)
    {
        hpUI.gameObject.SetActive(true);
        showHPStart = Time.time;

        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && slash.CanSlash)
        {
            OnDie(gameObject);
            return true;
        }
        else if (attack.Type == AttackType.ATTACK_TYPE_EXPLOSION)
        {
            if (slash.CanSlash)
            {
                StartCoroutine(LateDie());
            }
            else
            {
                currentBarrier = 0;
                slash.TriggerSlash();
            }
        }
        else
        {
            currentBarrier -= attack.Strength;
            if (currentBarrier > 0)
            {
                anim.SetTrigger("Hitted");
            }
            else
            {
                slash.TriggerSlash();
            }
        }

        return false;
    }

    IEnumerator LateDie()
    {
        yield return new WaitForSeconds(0.5f);
        OnDie(gameObject);
    }

    void OnDestroy()
    {
        if (hpUI)
        {
            Destroy(hpUI.gameObject);
        }
    }
}