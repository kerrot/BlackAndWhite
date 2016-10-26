using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlash : SingletonMonoBehaviour<PlayerSlash> {
    [SerializeField]
    private bool AutoSlash = false;
    [SerializeField]
    private float SlashSpeed = 5f;
    [SerializeField]
    private float maxSlashSpeed = 10f;
    [SerializeField]
    private float SlashStopRadius = 0.5f;
    [SerializeField]
    private float SlashAngle = 30;
    [SerializeField]
    private GameObject SlashRegion;
    [SerializeField]
    private GameObject SlashRegionDisplay;

    public float SlashRadius { get { return slashRadius; } }

    float slashRadius = 3f;
    int slashEndHash;
	int slashingHash;
    Animator anim;
    bool isSlashing = false;
    bool continueSlash = false;
    bool comboSlash = false;
    private GameObject TargetObject;
    float currentSpeed;

    private List<GameObject> slashList = new List<GameObject>();

    void Awake()
    {
        anim = GetComponent<Animator>();
        
        slashEndHash = Animator.StringToHash("PlayerBase.SlashEnd");
		slashingHash = Animator.StringToHash("PlayerBase.Slashing");
        InputController.OnMouseSingleClick += MultiSlash;

        slashRadius = SlashRegion.transform.localScale.x / 2;
        currentSpeed = SlashSpeed;
    }

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		if (info.fullPathHash == slashingHash)
        {
            if (TargetObject != null)
            {
                transform.LookAt(TargetObject.transform);

                Vector3 direction = transform.forward;
                direction.y = 0;
                transform.position += direction * currentSpeed * Time.deltaTime * PlayerTime.Instance.GetPlayerTimeFactor();

                if ((TargetObject.transform.position - transform.position).magnitude < SlashStopRadius)
                {
                    anim.SetTrigger("SlashEnd");
                    TargetObject = null;
                }
            }
        }

        SlashRegionDisplay.SetActive(slashList.Count > 0);
    }

    bool CanSlashEnemy(GameObject Enemy)
    {
        if (isSlashing == false && Enemy != null)
        {
            Vector3 direction = Enemy.transform.position - transform.position;
            EnemySlash battle = Enemy.GetComponent<EnemySlash>();
            if (direction.magnitude < slashRadius && battle.CanSlash)
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

    public bool SlashEnemy(GameObject Enemy)
	{
		if (CanSlashEnemy(Enemy)) {
            isSlashing = true;
			TargetObject = Enemy;

            PlayerMove.Instance.CanRotate = false;

            anim.SetBool("IsSlashing", true);
            anim.SetTrigger("Slash");

            return true;
        }

        return false;
	}

    void CheckSlash()
    {
        isSlashing = false;
        anim.SetBool("IsSlashing", false);
        PlayerMove.Instance.CanRotate = true;

        int count = 0;
        List<GameObject> list = PlayerBattle.Instance.Enemies.GetEnemy(transform.position, slashRadius, transform.rotation * Vector3.forward, SlashAngle);
        list.ForEach(o =>
        {
            EnemyBattle Enemy = o.GetComponent<EnemyBattle>();
            if (Enemy.Attacked(new Attack() { Type = AttackType.ATTACK_TYPE_SLASH }))
            {
                ++count;
            }
        });

        GameSystem.Instance.KillInOneTime(count);
        if (list.Count > 0 && comboSlash)
        {
            GameSystem.Instance.ComboSlash();
            comboSlash = false;
        }

		if (continueSlash || AutoSlash) {
			continueSlash = false;
            SlashNextTartget();
        }
    }

    void MultiSlash(Vector2 mousePosition)
    {
        if (continueSlash == false)
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.fullPathHash == slashEndHash)
            {
                if (isSlashing)
                {
                    continueSlash = true;
                }
                else
                {
                    SlashNextTartget();
                }
            }
        }
    }

    bool SlashNextTartget()
    {
        List<GameObject> list = PlayerBattle.Instance.Enemies.Enemies;
        foreach (GameObject o in list)
        {
            SlashEnemy(o);
            if (TargetObject == o)
            {
                comboSlash = true;

                if (anim.speed < 2.0f)
                {
                    anim.speed += 0.1f;
                }
                
                if (currentSpeed < maxSlashSpeed)
                {
                    currentSpeed += 0.5f;
                }

                return true;
            }
        }

		return false;
    }

    public void ResetSlashSpeed()
    {
        anim.speed = 1f;
        currentSpeed = SlashSpeed;
    }

	public void SkillSlash()
	{
		if (PlayerSkill.Instance.isSkill && isSlashing == false) 
		{
			if (SlashNextTartget ()) 
			{
				PlayerSkill.Instance.UsePower (1);
			}
		}
	}

	void OnDestroy()
	{
		InputController.OnMouseSingleClick -= MultiSlash;
	}
}
