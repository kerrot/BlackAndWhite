using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlash : SingletonMonoBehaviour<PlayerSlash> {
    public bool AutoSlash = false;
    public float SlashSpeed = 5;
    public float SlashStopRadius = 0.5f;
    public float SlashAngle = 30;
    public GameObject SlashRegion;

    float SlashRadius = 3f;
    int slashEndHash;
	int slashingHash;
    Animator anim;
    bool isSlashing = false;
    bool continueSlash = false;
    bool comboSlash = false;
    private GameObject TargetObject;

    void Awake()
    {
        anim = GetComponent<Animator>();

        slashEndHash = Animator.StringToHash("PlayerBase.SlashEnd");
		slashingHash = Animator.StringToHash("PlayerBase.Slashing");
        InputController.OnMouseSingleClick += MultiSlash;

        SlashRadius = SlashRegion.transform.localScale.x / 2;
    }
    void FixedUpdate()
    {
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		if (info.fullPathHash == slashingHash)
        {
            if (TargetObject != null)
            {
                transform.LookAt(TargetObject.transform);

                Vector3 direction = transform.forward;
                direction.y = 0;
                transform.position += direction * SlashSpeed * Time.deltaTime * PlayerTime.Instance.GetPlayerTimeFactor();

                if ((TargetObject.transform.position - transform.position).magnitude < SlashStopRadius)
                {
                    anim.SetTrigger("SlashEnd");
                    TargetObject = null;
                }
            }
        }
    }

    bool CanSlashEnermy(GameObject enermy)
    {
        if (isSlashing == false && enermy != null)
        {
            Vector3 direction = enermy.transform.position - transform.position;
            EnermyBattle battle = enermy.GetComponent<EnermyBattle>();
            if (direction.magnitude < SlashRadius && battle.CanSlash)
            {
                return true;
            }
        }

        return false;
    }

    public bool SlashEnermy(GameObject enermy)
	{
		if (CanSlashEnermy(enermy)) {
            isSlashing = true;
			TargetObject = enermy;

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

        GetComponent<TrailEffect>().EffectEnd();

        int count = 0;
        List<GameObject> list = PlayerBattle.Instance.Enermies.GetEnermy(transform.position, SlashRadius, transform.rotation * Vector3.forward, SlashAngle);
        list.ForEach(o =>
        {
            EnermyBattle enermy = o.GetComponent<EnermyBattle>();
            if (enermy.Attacked(new Attack() { Type = AttackType.ATTACK_TYPE_SLASH }))
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
        List<GameObject> list = PlayerBattle.Instance.Enermies.Enermies;
        foreach (GameObject o in list)
        {
            SlashEnermy(o);
            if (TargetObject == o)
            {
                comboSlash = true;
                return true;
            }
        }

		return false;
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
