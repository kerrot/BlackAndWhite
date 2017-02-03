using UniRx;
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
    bool canCombo = false;
    bool slashCombo = false;
    List<GameObject> slashList = new List<GameObject>();

    Vector3 slashRange;
    int EnemyMask;

    GameObject TargetObject;
    Vector3 TargetPosition;

    WhiteSkill skill;
    TrailEffect trail;
    PlayerTime playerTime;
    PlayerBattle battle;
    GameSystem system;

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

        trail = GetComponent<TrailEffect>();
        playerTime = GetComponent<PlayerTime>();
        battle = GetComponent<PlayerBattle>();
        system = GameObject.FindObjectOfType<GameSystem>();
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
                TargetPosition.y = 0f;
            }

            transform.LookAt(TargetPosition);

            if (isSlashing && !slashReach && Vector3.Distance(TargetPosition, transform.position) < SlashStopRadius)
            {
                anim.SetTrigger("SlashEnd");

                slashReach = true;
                canCombo = true;

                TargetObject = null;
            }
        }
        else if (info.fullPathHash == slashEndHash)
        {
            if (comboHint)
            {
                comboHint.SetActive(FindSlashEnemy() != null && canCombo);

                if (comboHint.activeSelf)
                {
                    if (slashCombo || AutoSlash)
                    {
                        MultiSlash(Input.mousePosition);
                    }
                }
            }
        }
        else
        {
            canCombo = false;
            slashCombo = false;
            if (comboHint)
            {
                comboHint.SetActive(false);
            }

            if (!isSlashing)
            {
                SlashSpeedUp(0f);
            }
        }

        SlashRegionDisplay.SetActive(slashList.Count > 0);
    }

    bool CanSlashEnemy(GameObject Enemy)
    {
        if (Enemy != null)
        {
            EnemySlash slash = Enemy.GetComponent<EnemySlash>();
            if (Vector3.Distance(Enemy.transform.position, transform.position) < SlashRadius && slash && slash.CanSlash)
            {
                return true;
            }
        }

        return false;
    }

    bool CanSlash()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        return PlayerBattle.IsDead == false && isSlashing == false && info.fullPathHash != slashEndHash;
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
        MultiSlash(Input.mousePosition);

        return SlashEnemy(FindSlashEnemy());
    }

    GameObject FindSlashEnemy()
    {
        GameObject obj = null;

        if (slashList.Count > 0)
        {
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
        }

        return obj;
    }

    public bool SlashEnemy(GameObject Enemy)
    {
        if (CanSlash() && CanSlashEnemy(Enemy))
        {
            DoSlash(Enemy);
            return true;
        }

        return false;
    }

    bool DoSlash(GameObject Enemy)
    {
        if (Enemy == null)
        {
            return false;
        }

        isSlashing = true;
        slashReach = false;
        slashCombo = false;

        TargetObject = Enemy;

        PlayerMove.CanRotate = false;

        anim.SetBool("IsSlashing", true);
        anim.SetTrigger("Slash");

        trail.SlashTrailStart();

        if (skill && skill.Activated())
        {
            SlashSpeedUp(slashSpeed = maxSpeedup);
        }

        return true;
    }

    void CheckSlash()
    {
        int count = 0;
        Collider[] enemies = Physics.OverlapBox(SlashRegion.transform.position, slashRange, SlashRegion.transform.rotation, EnemyMask);
        if (battle)
        {
            List<EnemyBattle> tmp = new List<EnemyBattle>();

            enemies.ToList().ForEach(e =>
            {
                EnemySlash Enemy = e.gameObject.GetComponent<EnemySlash>();
                if (Enemy && Enemy.CanSlash)
                {
                    tmp.Add(e.gameObject.GetComponent<EnemyBattle>());
                }
            });

            tmp.ForEach(t =>
            {
                if (t && t.Attacked(battle, battle.CreateAttack(AttackType.ATTACK_TYPE_SLASH, strength, force)))
                {
                    ++count;
                }
            });
        }

        if (count > 0)
        {
            slashCount.OnNext(count);
        }

        AudioHelper.PlaySE(gameObject, slashSE);
        isSlashing = false;
        anim.SetBool("IsSlashing", false);
        anim.SetBool("IsMove", false);
        PlayerMove.CanRotate = true;
        comboHint.SetActive(FindSlashEnemy() != null && canCombo);
    }

    void MultiSlash(Vector2 mousePosition)
    {
        if (!slashCombo)
        {
            if (comboHint && comboHint.activeSelf)
            {
                slashCombo = true;
            }
        }

        if (!isSlashing && slashCombo)
        {
            if (DoSlash(FindSlashEnemy()))
            {
                slashSpeed += speedup;
                if (slashSpeed > maxSpeedup)
                {
                    slashSpeed = maxSpeedup;
                }
                SlashSpeedUp(slashSpeed);

                comboSlash.OnNext(Unit.Default);
            }
        }
    }

    void ComboEnd()
    {
        canCombo = false;

        if (!isSlashing)
        {
            if (slashCombo)
            {
                MultiSlash(Input.mousePosition);
            }
            else
            {
                SlashSpeedUp(0f);
                if (system)
                {
                    system.ResetSlashCount();
                }
            }
        }
    }

    public void SlashSpeedUp(float speed)
    {
        if (playerTime)
        {
            playerTime.SpeedChange(slashSpeed = speed, this);
        }
    }
}
