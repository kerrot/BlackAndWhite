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
    [SerializeField]
    private MeshRenderer Lance;
    [SerializeField]
    private GameObject LanceEffect;
    [SerializeField]
    private GameObject BlueSkill;

    public delegate void SkillAction(bool usingSkill, float power, float maxPower);
    public static SkillAction OnPowerChnaged;

    private int power;

	public bool isSkill { get { return usingSkill; } }
    private bool usingSkill = false;

    private float skillStartTime;

    Material lanceEffectmat;

    void Start()
    {
		SkillBtn btn = GameObject.FindObjectOfType<SkillBtn> ();
		if (btn) 
		{
			btn.OnBlueChanged += BlueAttribute;
            btn.OnRedChanged += RedAttribute;
            btn.OnGreenChanged += GreenAttribute;
            btn.OnPowerUsed += UseSkill;
        }

        lanceEffectmat = LanceEffect.GetComponentInChildren<MeshRenderer>().material;

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
    //    if (usingSkill) 
    //    {
    //        if (Time.realtimeSinceStartup - skillStartTime > powerCostTime) 
    //        {
    //            skillStartTime += powerCostTime;
				//UsePower (1);
    //        }
    //    }

        //if (Input.GetButtonDown("Jump"))
    }

    void UseSkill()
    {
        ElementType type = GetComponent<Attribute>().Type;
        switch (type)
        {
            case ElementType.ELEMENT_TYPE_BLUE:
                BlueSkill.SetActive(true);
                break;
        }
    }

    //public void WhiteSkill()
    //{
    //    if (!usingSkill) 
    //    {
    //        PlayerTime.Instance.SlowMotion(0.2f, 0.5f);
    //        skillStartTime = Time.realtimeSinceStartup;
    //        CameraEffect.Instance.WhiteSkillEffect(true);
    //        usingSkill = true;
    //    }
    //}

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
            CameraEffect.Instance.WhiteSkillEffect(false);
            usingSkill = false;
		}

		UpdatePower();
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

        UpdatePower();
    }

    void UpdatePower() 
    {
        if (OnPowerChnaged != null)
        {
            OnPowerChnaged(usingSkill, power, maxPower);
        }
    }

	void BlueAttribute(bool active)
	{
        AttributeChange(active, new Color(0f, 0f, 2f), ElementType.ELEMENT_TYPE_BLUE);
    }
    void RedAttribute(bool active)
    {
        AttributeChange(active, new Color(2f, 0f, 0f), ElementType.ELEMENT_TYPE_RED);
    }
    void GreenAttribute(bool active)
    {
        AttributeChange(active, new Color(0f, 2f, 0f), ElementType.ELEMENT_TYPE_GREEN);
    }

    void AttributeChange(bool active, Color color, ElementType type)
    {
        LanceEffect.gameObject.SetActive(active);
        if (active)
        {
            Lance.material.SetColor("_EmissionColor", color);
            lanceEffectmat.SetColor("_EmissionColor", color);
        }
        else
        {
            Lance.material.SetColor("_EmissionColor", Color.black);
            lanceEffectmat.SetColor("_EmissionColor", Color.black);
        }

        Attribute attr = GetComponent<Attribute>();
        if (attr)
        {
            attr.SetElement((active) ? type : ElementType.ELEMENT_TYPE_NONE);
        }
    }
}
