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

    public float SlashRadius { get { return slashRadius; } }
    public bool IsSlashing { get { return isSlashing; } }

    private Subject<int> slashCount = new Subject<int>();
    public IObservable<int> OnSlashCount { get { return slashCount; } }

    private Subject<Unit> comboSlash = new Subject<Unit>();
    public IObservable<Unit> OnComboSlash { get { return comboSlash; } }

    float slashRadius;
    float slashSpeed = 0f;

    int shiftHash;
    int slashEndHash;
	int slashingHash;
    Animator anim;
    bool isSlashing = false;
    bool slashReach = false;
    bool continueSlash = false;

    List<GameObject> slashList = new List<GameObject>();

    Vector3 slashRange;
    int EnemyMask;

    GameObject TargetObject;
    Vector3 TargetPosition;

    WhiteAura aura;

    void Awake()
    {
        anim = GetComponent<Animator>();

        shiftHash = Animator.StringToHash("PlayerBase.shift");
        slashEndHash = Animator.StringToHash("PlayerBase.SlashEnd");
		slashingHash = Animator.StringToHash("PlayerBase.Slashing");
        InputController.OnMouseDown.Subscribe(p => MultiSlash(p)).AddTo(this);
        InputController.OnSlashClick.Subscribe(u => Slash()).AddTo(this);

        slashRadius = SlashRegionDisplay.GetComponent<SphereCollider>().radius;
        slashRange = SlashRegion.GetComponent<BoxCollider>().size / 2.0f;

        EnemyMask = LayerMask.GetMask("Enemy");
        aura = GameObject.FindObjectOfType<WhiteAura>();
    }

    void Start()
    {
        EnemyGenerator.OnEnemyClicked.Subscribe(o => SlashEnemy(o)).AddTo(this);
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		//if (info.fullPathHash == slashingHash)
  //      {
  //          if (TargetObject)
  //          {
  //              TargetPosition = TargetObject.transform.position;
  //          }

  //          transform.LookAt(TargetPosition);

  //          if (isSlashing && !slashReach && Vector3.Distance(TargetPosition, transform.position) < SlashStopRadius)
  //          {
  //              anim.SetTrigger("SlashEnd");
  //              slashReach = true;
  //              TargetObject = null;
  //          }
  //      }
  //      else 
        if (info.fullPathHash == shiftHash)
        {
            if (TargetObject)
            {
                TargetPosition = TargetObject.transform.position;
            }

            if (AutoSlash || (aura && aura.IsAura))
            {
                continueSlash = true;
            }

            if (isSlashing == false && continueSlash)
            {
                Slash();
            }
        }
        else
        {
            continueSlash = false;
        }

        SlashRegionDisplay.SetActive(slashList.Count > 0);
    }

    public bool CanSlashEnemy(GameObject Enemy)
    {
        if (isSlashing == false && Enemy != null)
        {
            Vector3 direction = Enemy.transform.position - transform.position;
            EnemySlash battle = Enemy.GetComponent<EnemySlash>();
            if (direction.magnitude < SlashRadius && battle.CanSlash)
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

    bool Slash()
    {
        if (slashList.Count > 0)
        {
            EnemyGenerator enemies = GameObject.FindObjectOfType<EnemyGenerator>();
            if (enemies)
            {
                GameObject obj = null;
                float min = Mathf.Infinity;
                EnemyGenerator.Enemies.ForEach(e =>
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
        }

        return false;
    }

    public bool SlashEnemy(GameObject Enemy)
	{
        if (isSlashing)
        {
            MultiSlash(Input.mousePosition);
            return true;
        }

		if (CanSlashEnemy(Enemy)) {
            isSlashing = true;
            slashReach = false;
            TargetObject = Enemy;

            TargetPosition = Enemy.transform.position;
            transform.LookAt(TargetObject.transform);

            PlayerMove move = GameObject.FindObjectOfType<PlayerMove>();
            if (move)
            {
                move.CanRotate = false;
            }

            //anim.SetBool("IsSlashing", true);
            //anim.SetTrigger("Slash");
            anim.SetTrigger("Shift");

            return true;
        }

        return false;
	}

    void CheckSlash()
    {
        AudioHelper.PlaySE(gameObject, slashSE);
        isSlashing = false;
        anim.SetBool("IsSlashing", false);
        anim.SetBool("IsMove", false);
        PlayerMove move = GameObject.FindObjectOfType<PlayerMove>();
        if (move)
        {
            move.CanRotate = true;
        }

        SlashEffect shadow = GetComponent<SlashEffect>();
        if (shadow)
        {
            shadow.ShadowEffect();
        }

        transform.position = (transform.position - TargetPosition).normalized * SlashStopRadius + TargetPosition;

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
                if (Enemy && Enemy.Attacked(battle, battle.CreateAttack(AttackType.ATTACK_TYPE_SLASH, strength)))
                {
                    ++count;
                }
            });
        }

        if (count > 0)
        {
            slashCount.OnNext(count);
            if (continueSlash)
            {
                comboSlash.OnNext(Unit.Default);
            }
        }
    }

    void MultiSlash(Vector2 mousePosition)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash == slashEndHash)
        {
            continueSlash = true;
        }
    }

    public void SlashSpeedUp(float speed)
    {
        PlayerTime time = GetComponent<PlayerTime>();
        if (time)
        {
            time.SpeedChange(slashSpeed = speed, this);
        }
    }
}
