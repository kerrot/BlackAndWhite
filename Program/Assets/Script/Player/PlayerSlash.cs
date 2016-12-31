﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerSlash : MonoBehaviour {
    [SerializeField]
    private bool AutoSlash = false;
    [SerializeField]
    private float speedup;
    [SerializeField]
    private float maxSpeedup;
    [SerializeField]
    private float SlashStopRadius;
    [SerializeField]
    private GameObject SlashRegion;
    [SerializeField]
    private GameObject SlashRegionDisplay;
    [SerializeField]
    private AudioClip slashSE;
    [SerializeField]
    private float strength;
    [SerializeField]
    private float force;
    [SerializeField]
    private GameObject comboHint;

    public float SlashRadius { get { return slashRadius; } }
    public bool IsSlashing { get { return isSlashing; } }

    private Subject<int> slashCount = new Subject<int>();
    public IObservable<int> OnSlashCount { get { return slashCount; } }

    private Subject<Unit> comboSlash = new Subject<Unit>();
    public IObservable<Unit> OnComboSlash { get { return comboSlash; } }

    float slashRadius;
    float slashSpeed = 0f;

    int slashEndHash;
	int slashingHash;
    Animator anim;
    bool isSlashing = false;
    bool slashReach = false;
    bool continueSlash = false;
    bool slashComboStart;
    List<GameObject> slashList = new List<GameObject>();

    Vector3 slashRange;
    int EnemyMask;

    GameObject TargetObject;
    Vector3 TargetPosition;

    WhiteSkill skill;

    void Awake()
    {
        anim = GetComponent<Animator>();
        
        slashEndHash = Animator.StringToHash("PlayerBase.SlashEnd");
		slashingHash = Animator.StringToHash("PlayerBase.Slashing");
        InputController.OnMouseDown.Subscribe(p => MultiSlash(p)).AddTo(this);
        InputController.OnSlashClick.Subscribe(u => Slash()).AddTo(this);

        slashRadius = SlashRegionDisplay.GetComponent<SphereCollider>().radius;
        slashRange = SlashRegion.GetComponent<BoxCollider>().size / 2.0f;

        EnemyMask = LayerMask.GetMask("Enemy");
        skill = GameObject.FindObjectOfType<WhiteSkill>();

        if (!comboHint)
        {
            comboHint = GameObject.Find("ComboHint");
        }
        comboHint.SetActive(false);
    }

    void Start()
    {
        EnemyManager.OnEnemyClicked.Subscribe(o => SlashEnemy(o)).AddTo(this);
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		if (info.fullPathHash == slashingHash)
        {
            if (TargetObject)
            {
                TargetPosition = TargetObject.transform.position;
            }

            transform.LookAt(TargetPosition);

            if (isSlashing && !slashReach && Vector3.Distance(TargetPosition, transform.position) < SlashStopRadius)
            {
                anim.SetTrigger("SlashEnd");
                slashReach = true;
                if (comboHint)
                {
                    comboHint.SetActive(true);
                }
                TargetObject = null;
            }
        }
        else if (info.fullPathHash == slashEndHash)
        {
            if (comboHint && comboHint.activeSelf)
            {
                if (AutoSlash || (skill && skill.Activated()))
                {
                    continueSlash = true;
                }
            }
            
            if (isSlashing == false && continueSlash)
            {
                Slash();
            }
        }
        else
        {
            continueSlash = false;
            if (comboHint)
            {
                comboHint.SetActive(false);
            }
        }

        SlashRegionDisplay.SetActive(slashList.Count > 0);
    }

    public bool CanSlashEnemy(GameObject Enemy)
    {
        if (PlayerBattle.IsDead)
        {
            return false;
        }

        if (isSlashing == false && Enemy != null)
        {
            Vector3 direction = Enemy.transform.position - transform.position;
            EnemySlash slash = Enemy.GetComponent<EnemySlash>();
            if (direction.magnitude < SlashRadius && slash && slash.CanSlash)
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterSlashObject(GameObject obj, bool canSlash)
    {
        if (canSlash)
        {
            if (!slashList.Contains(obj))
            {
                slashList.Add(obj);
            }
        }
        else
        {
            slashList.Remove(obj);
        }
    }

    public bool Slash()
    {
        if (slashList.Count > 0)
        {
            GameObject obj = null;
            float min = Mathf.Infinity;
            EnemyManager.Enemies.ForEach(e =>
            {
                if (CanSlashEnemy(e))
                {
                    float tmp = Vector3.Distance(transform.position, e.transform.position);
                    if (tmp < min)
                    {
                        obj = e;
                        min = tmp;
                    }
                }
            });

            return SlashEnemy(obj);
        }

        return false;
    }

    public bool SlashEnemy(GameObject Enemy)
	{
        if (comboHint && comboHint.activeSelf)
        {
            MultiSlash(Input.mousePosition);
            return true;
        }

		if (CanSlashEnemy(Enemy)) {
            isSlashing = true;
            slashReach = false;
            TargetObject = Enemy;

            PlayerMove.CanRotate = false;

            anim.SetBool("IsSlashing", true);
            anim.SetTrigger("Slash");

            if (skill && skill.Activated())
            {
                SlashSpeedUp(slashSpeed = maxSpeedup);
            }

            return true;
        }

        return false;
	}

    void CheckSlash()
    {
        if (slashSpeed < maxSpeedup)
        {
            SlashSpeedUp(slashSpeed += speedup);
        }

        int count = 0;
        Collider[] enemies = Physics.OverlapBox(SlashRegion.transform.position, slashRange, SlashRegion.transform.rotation, EnemyMask);

        PlayerBattle battle = GetComponent<PlayerBattle>();
        if (battle)
        {
            enemies.ToList().ForEach(e =>
            {
                EnemyBattle Enemy = e.gameObject.GetComponent<EnemyBattle>();
                if (Enemy && Enemy.Attacked(battle, battle.CreateAttack(AttackType.ATTACK_TYPE_SLASH, strength, force)))
                {
                    ++count;
                }
            });
        }

        if (count > 0)
        {
            slashCount.OnNext(count);
            if (continueSlash && slashComboStart)
            {
                comboSlash.OnNext(Unit.Default);
            }

            slashComboStart = true;
        }

        AudioHelper.PlaySE(gameObject, slashSE);
        isSlashing = false;
        anim.SetBool("IsSlashing", false);
        anim.SetBool("IsMove", false);
        PlayerMove.CanRotate = true;
    }

    void MultiSlash(Vector2 mousePosition)
    {
        if (comboHint && comboHint.activeSelf)
        {
            continueSlash = true;
        }
    }

    void ComboEnd()
    {
        if (comboHint)
        {
            comboHint.SetActive(false);
        }
    }

    public void SlashSpeedUp(float speed)
    {
        if (speed == 0)
        {
            slashComboStart = false;
        }

        PlayerTime time = GetComponent<PlayerTime>();
        if (time)
        {
            time.SpeedChange(slashSpeed = speed, this);
        }
    }
}
