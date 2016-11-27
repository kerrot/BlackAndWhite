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
	[SerializeField]
	private GameObject RedSkill;
    [SerializeField]
    private GameObject SkillPos;
    [SerializeField]
    private GameObject GreenSkill;
    [SerializeField]
    private GameObject MagentaSkill;
	[SerializeField]
	private GameObject CyanSkill;
    [SerializeField]
    private GameObject YellowSkill;

    //	private ReactiveProperty<int> power;
    //
    //	public bool isSkill { get { return usingSkill; } }
    //    private bool usingSkill = false;
    //
    //    private float skillStartTime;

    Material lanceEffectmat;

    ElementType castingType;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        SkillBtn btn = GameObject.FindObjectOfType<SkillBtn> ();
		if (btn) 
		{
			btn.OnBlueChanged += BlueAttribute;
            btn.OnRedChanged += RedAttribute;
            btn.OnGreenChanged += GreenAttribute;
            btn.OnPowerUsed += UseSkill;
        }

        lanceEffectmat = LanceEffect.GetComponentInChildren<MeshRenderer>().material;

        //this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

//    void UniRxUpdate()
//    {
//        if (usingSkill) 
//        {
//            if (Time.realtimeSinceStartup - skillStartTime > powerCostTime) 
//            {
//                skillStartTime += powerCostTime;
//				UsePower (1);
//            }
//        }
//
//        if (Input.GetButtonDown("Jump"))
//    }

    void UseSkill()
    {
        castingType = GetComponent<Attribute>().Type;
        if (castingType == ElementType.ELEMENT_TYPE_YELLOW)
        {
            YellowDebuff debuff = GameObject.FindObjectOfType<YellowDebuff>();
            if (debuff)
            {
                debuff.End();
                transform.position = debuff.transform.position;
                return;
            }
        }

        anim.SetTrigger("Skill");
    }

    void DoSkill()
    {
        switch (castingType)
        {
            case ElementType.ELEMENT_TYPE_BLUE:
                BlueSkill.SetActive(true);
	            break;
			case ElementType.ELEMENT_TYPE_RED:
                Instantiate(RedSkill, SkillPos.transform.position, transform.rotation);
                break;
            case ElementType.ELEMENT_TYPE_GREEN:
                Instantiate(GreenSkill, transform.position, Quaternion.identity);
                break;
            case ElementType.ELEMENT_TYPE_MAGENTA:
                MagentaSkill.SetActive(true);
                break;
			case ElementType.ELEMENT_TYPE_CYAN:
				Instantiate(CyanSkill, transform.position, Quaternion.identity);
				break;
            case ElementType.ELEMENT_TYPE_YELLOW:
                Instantiate(YellowSkill, SkillPos.transform.position, transform.rotation);
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

//    public void PowerUsed(int v)
//	{
//		if (v <= 0) 
//		{
//			return;
//		}
//
//		power -= v;
//		if (power < 0) 
//		{
//			power = 0;
//		}
//
//		if (usingSkill && power == 0) 
//		{
//			PlayerTime.Instance.SlowMotion(1, 1);
//            CameraEffect.Instance.WhiteSkillEffect(false);
//            usingSkill = false;
//		}
//
//		UpdatePower();
//	}

//    public void AddPower(int v)
//    {
//		if (v <= 0) 
//		{
//			return;
//		}
//
//        power += v;
//        if (power > maxPower)
//        {
//            power = maxPower;
//        }
//
//        UpdatePower();
//    }

//    void UpdatePower() 
//    {
//        if (OnPowerChnaged != null)
//        {
//            OnPowerChnaged(usingSkill, power, maxPower);
//        }
//    }

	void BlueAttribute(bool active)
	{
        AttributeChange(active, ElementType.ELEMENT_TYPE_BLUE);
    }
    void RedAttribute(bool active)
    {
        AttributeChange(active, ElementType.ELEMENT_TYPE_RED);
    }
    void GreenAttribute(bool active)
    {
        AttributeChange(active, ElementType.ELEMENT_TYPE_GREEN);
    }

    void AttributeChange(bool active, ElementType type)
    {
        LanceEffect.gameObject.SetActive(active);

        Attribute attr = GetComponent<Attribute>();
        if (attr)
        {
            attr.SetElement(active, type);

            Color color = Attribute.GetColor(attr.Type) * 2;
            Lance.material.SetColor("_EmissionColor", color);
            lanceEffectmat.SetColor("_EmissionColor", color);
        }
    }
}
