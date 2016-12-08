using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class PlayerSkill : MonoBehaviour
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
    [SerializeField]
    private WhiteAura WhiteSkill;

    private BoolReactiveProperty canSkill = new BoolReactiveProperty();
    private BoolReactiveProperty redOn = new BoolReactiveProperty();
    private BoolReactiveProperty greenOn = new BoolReactiveProperty();
    private BoolReactiveProperty blueOn = new BoolReactiveProperty();
    public IObservable<bool> CanSkill { get { return canSkill; } }
    public IObservable<bool> RedOn { get { return redOn; } }
    public IObservable<bool> GreenOn { get { return greenOn; } }
    public IObservable<bool> BlueOn { get { return blueOn; } }

    public ElementType CurrentElement
	{
		get 
		{
			Attribute attr = GetComponent<Attribute>();
			if (attr)
			{
				return attr.Type;
			}

			return ElementType.ELEMENT_TYPE_NONE;
		}
	}

    //	private ReactiveProperty<int> power;
    //
    //	public bool isSkill { get { return usingSkill; } }
    //    private bool usingSkill = false;
    //
    //    private float skillStartTime;

    Material lanceEffectmat;

    ElementType castingType;

    Animator anim;
	SkillBtn btn;

	void Awake()
	{
        lanceEffectmat = LanceEffect.GetComponentInChildren<MeshRenderer>().material;

        btn = GameObject.FindObjectOfType<SkillBtn> ();
		if (btn) 
		{
			btn.OnRedClick.Subscribe (v => AttributeChange(v, ElementType.ELEMENT_TYPE_RED)).AddTo(this);
			btn.OnGreenClick.Subscribe (v => AttributeChange(v, ElementType.ELEMENT_TYPE_GREEN)).AddTo(this);
			btn.OnBlueClick.Subscribe (v => AttributeChange(v, ElementType.ELEMENT_TYPE_BLUE)).AddTo(this);
			btn.OnPowerClick.Subscribe (u => UseSkill()).AddTo(this);
		}
	}

    void Start()
    {
        anim = GetComponent<Animator>();

        
		
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
        if (canSkill.Value)
        {
            if (castingType == ElementType.ELEMENT_TYPE_YELLOW)
            {
                YellowDebuff debuff = GameObject.FindObjectOfType<YellowDebuff>();
                if (debuff)
                {
                    debuff.End();
                    return;
                }
            }

            anim.SetTrigger("Skill");
        }
    }

    void DoSkill()
    {
        switch (castingType)
        {
            case ElementType.ELEMENT_TYPE_BLUE:
                BlueSkill.SetActive(true);
	            break;
			case ElementType.ELEMENT_TYPE_RED:
                Instantiate(RedSkill, SkillPos.transform.position, FindNearestDirection());
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
                Instantiate(YellowSkill, SkillPos.transform.position, FindNearestDirection());
                break;
            case ElementType.ELEMENT_TYPE_WHITE:
                WhiteSkill.Launch();
                break;
        }
    }

    Quaternion FindNearestDirection()
    {
        float distance = Mathf.Infinity;
        GameObject min = gameObject;
        EnemyGenerator.Enemies.ForEach(e =>
        {
            Collider c = e.GetComponent<Collider>();
            if (c && c.enabled && !c.isTrigger)
            {
                float tmpDistance = Vector3.Distance(e.transform.position, transform.position);
                float tmpAngle = Mathf.Abs(Vector3.Angle(transform.forward, e.transform.position - transform.position));
                if (tmpAngle < 10 && tmpDistance < distance)
                {
                    distance = tmpDistance;
                    min = e;
                }
            }
        });

        if (distance != Mathf.Infinity)
        {
            return Quaternion.LookRotation(min.transform.position - transform.position);
        }

        return transform.rotation;
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

    void AttributeChange(bool active, ElementType type)
    {
        LanceEffect.gameObject.SetActive(active);

        Attribute attr = GetComponent<Attribute>();
        if (attr)
        {
            attr.SetElement(active, type);

            Color color = Attribute.GetColor(attr.Type, 0.5f) * 2;
            Lance.material.SetColor("_EmissionColor", color);
            lanceEffectmat.SetColor("_EmissionColor", color);
        }
    }
}
