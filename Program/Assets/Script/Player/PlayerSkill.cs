using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class PlayerSkill : SingletonMonoBehaviour<PlayerSkill>
{
    [SerializeField]
    private float powerCostTime;
    [SerializeField]
    private int maxPower;

    private int power;

	public bool isSkill { get { return usingSkill; } }
    private bool usingSkill = false;

    private float skillStartTime;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        if (usingSkill) 
        {
            if (Time.realtimeSinceStartup - skillStartTime > powerCostTime) 
            {
                skillStartTime += powerCostTime;
				UsePower (1);
            }
        }
    }

    public void WhiteSkill()
    {
        if (!usingSkill) 
        {
            PlayerTime.Instance.SlowMotion(0.2f, 0.5f);
            skillStartTime = Time.realtimeSinceStartup;
            usingSkill = true;
        }
    }

	public void UsePower(int v)
	{
		if (v <= 0) 
		{
			return;
		}

		power -= v;
		if (power < 0) 
		{
			power = 0;
		}

		if (usingSkill && power == 0) 
		{
			PlayerTime.Instance.SlowMotion(1, 1);
			usingSkill = false;
		}

		UpdatePowerUI();
	}

    public void AddPower(int v)
    {
		if (v <= 0) 
		{
			return;
		}

        power += v;
        if (power > maxPower)
        {
            power = maxPower;
        }

        UpdatePowerUI();
    }

    void UpdatePowerUI() 
    {
		SkillUI.Instance.UpdatePowerBar(usingSkill, power, maxPower);
    }
}
