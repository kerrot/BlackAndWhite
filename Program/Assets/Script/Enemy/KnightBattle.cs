﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class KnightBattle : UnitBattle {

    [SerializeField]
    private float HPMax;
    [SerializeField]
    private float deadTime;
    [SerializeField]
    private float showHPTime;
    [SerializeField]
    private Transform HPUICenter;
    [SerializeField]
    private GameObject wanderEffect;

    private Subject<GameObject> dieSubject = new Subject<GameObject>();

    public IObservable<GameObject> OnDie { get { return dieSubject; } }


    private FloatReactiveProperty currentHP = new FloatReactiveProperty(HPMax);

    HPBarUI hpUI;
    float showHPStart;

    Animator anim;

    float deadStart;

    Collider coll;

    int wanderHash;
    int damageHash;

    bool dead;
    PlayerBattle player;
    Attribute attr;

    void Awake()
    {
        attr = GetComponent<Attribute>();

        player = GameObject.FindObjectOfType<PlayerBattle>();
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        wanderHash = Animator.StringToHash("EnemyBase.Wander");

        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            GameObject tmp = ui.CreateHPUI();
            hpUI = tmp.GetComponent<HPBarUI>();
            hpUI.SetBarrierMax(HPMax);
            hpUI.SetBarrierCurrent(currentHP.Value);
            hpUI.SetBarrierMax(0);
            hpUI.SetBarrierCurrent(0);
            hpUI.SetRecoverMax(deadTime);
            hpUI.SetRecoverCurrent(currentRecover);
            hpUI.SetRecoverEnable(false);
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
        this.OnAnimatorMoveAsObservable().Subscribe(_ => UniRxAnimatorMove());
    }

    void UniRxAnimatorMove()
    {
        transform.position = anim.rootPosition;
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

        if (currentHP <= 0 && Time.time - deadStart > deadTime)
        {
            currentHP = HPMax;
            coll.enabled = true;
            anim.SetTrigger("Revive");
            hpUI.SetBarrierEnable(true);
        }


        if (player)
        {
            anim.SetBool("Wander", player.Missing);
            wanderEffect.SetActive(player.Missing);
        }
    }

    void UpdateUI()
    {
        hpUI.SetHPCurrent(currentHP);
        hpUI.SetBarrierCurrent(currentBarrier);
        hpUI.SetRecoverCurrent(currentRecover);
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        if (dead)
        {
            return false;
        }

        #region ShowHP
        hpUI.gameObject.SetActive(true);
        showHPStart = Time.time;
        #endregion

        #region Modify with Attribute
        Attribute attr = GetComponent<Attribute>();
        if (attr && attr.ProcessAttack(unit, attack))
        {
            return false;
        }
        #endregion

        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && slash.CanSlash)
        {
            Die(attack);
            return true;
        }
        else if (attack.Type == AttackType.ATTACK_TYPE_EXPLOSION)
        {
            if (slash.CanSlash)
            {
                //anim.SetTrigger("Hitted");
                //GetComponent<Collider>().enabled = false;

                //Observable.FromCoroutine(_ => LateDie(attack)).Subscribe().AddTo(this);
                slash.TriggerSlash();
            }
            else
            {
                currentBarrier = 0;
                slash.TriggerSlash();
                explosionAttacked.OnNext(gameObject);
            }
        }
        else if (currentBarrier <= 0)
        {
            currentHP -= attack.Strength;
            if (currentHP <= 0)
            {
                coll.enabled = false;
                anim.SetTrigger("Die");
                deadStart = Time.time;
                hpUI.SetBarrierEnable(false);
            }
        }
        else
        {
            currentBarrier -= attack.Strength;
            if (currentBarrier > 0)
            {
                if (attack.Type == AttackType.ATTACK_TYPE_SKILL)
                {
                    if (attack.Element == ElementType.ELEMENT_TYPE_BLUE)
                    {
                        anim.SetTrigger("Tumble");
                        AudioHelper.PlaySE(gameObject, tumbleSE);
                    }
                    else if (attack.Element == ElementType.ELEMENT_TYPE_RED)
                    {
                        anim.SetTrigger("Fire");
                        AudioHelper.PlaySE(gameObject, fireSE);
                    }
                    else if (attack.Element == ElementType.ELEMENT_TYPE_GREEN)
                    {
                        anim.SetTrigger("Hitted");
                        AudioHelper.PlaySE(gameObject, woodSE);
                    }
                    else if (attack.Element == ElementType.ELEMENT_TYPE_YELLOW)
                    {
                        anim.SetTrigger("DamageStart");
                    }
                    else
                    {
                        anim.SetTrigger("Hitted");
                    }
                }
                else
                {
                    AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                    if (info.fullPathHash == wanderHash)
                    {
                        anim.SetTrigger("Frighten");
                        AudioHelper.PlaySE(gameObject, frightenSE);
                    }
                    else
                    {
                        anim.SetTrigger("Hitted");
                    }
                }
            }
            else
            {
                slash.TriggerSlash();
            }
        }

        return false;
    }

    public void RecoverFromDamage()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash == damageHash)
        {
            anim.SetTrigger("DamageEnd");
        }
    }

    void Die(Attack attack)
    {
        if (DeadAction)
        {
            DeadAction act = DeadAction.GetComponent<DeadAction>();
            if (act)
            {
                act.Attacker = this;
                act.Atk = new Attack() { Type = AttackType.ATTACK_TYPE_EXPLOSION, 
                                        Element = (attr && Attribute.isBase(attr.Type) ? attr.Type : attack.Element };
            }
        }

        
        if (attr && Attribute.isBase(attr.Type))
        {
            ProduceEnergyPeace(attr.Type, 2);
        }
        else
        {
            if ((attack.Element & ElementType.ELEMENT_TYPE_RED) != 0)
            {
                ProduceEnergyPeace(ElementType.ELEMENT_TYPE_RED, 1);
            }
            if ((attack.Element & ElementType.ELEMENT_TYPE_GREEN) != 0)
            {
                ProduceEnergyPeace(ElementType.ELEMENT_TYPE_GREEN, 1);
            }
            if ((attack.Element & ElementType.ELEMENT_TYPE_BLUE) != 0)
            {
                ProduceEnergyPeace(ElementType.ELEMENT_TYPE_BLUE, 1);
            }
        }

        dead = true;

        dieSubject.OnNext(gameObject);
    }

    void ProduceEnergyPeace(ElementType ele, int num)
    {
        if (energyPeace)
        {
            for (int i = 0; i < num; ++i)
            {
                GameObject obj = Instantiate(energyPeace, HPUICenter.transform.position, Quaternion.identity) as GameObject;
                EnergyPeace peace = obj.GetComponent<EnergyPeace>();
                if (peace)
                {
                    peace.Type = ele;
                }
            }
        }
    }

    IEnumerator LateDie(Attack attack)
    {
        yield return new WaitForSeconds(0.5f);
        Die(attack);
    }

    void UniRxOnDestroy()
    {
        if (hpUI)
        {
            Destroy(hpUI.gameObject);
        }

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}