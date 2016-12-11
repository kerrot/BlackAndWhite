using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField]
    private float maxEnergy;
    [SerializeField]
    private float baseCost;
    [SerializeField]
    private float costCheckTime;
    [SerializeField]
    private Skill[] skills;


    private BoolReactiveProperty canSkill = new BoolReactiveProperty();
    private FloatReactiveProperty redEnergy = new FloatReactiveProperty();
    private FloatReactiveProperty greenEnergy = new FloatReactiveProperty();
    private FloatReactiveProperty blueEnergy = new FloatReactiveProperty();
    private Dictionary<ElementType, FloatReactiveProperty> mapping = new Dictionary<ElementType, FloatReactiveProperty>();
    public FloatReactiveProperty RedEnergy { get { return redEnergy; } }
    public FloatReactiveProperty GreenEnergy { get { return greenEnergy; } }
    public FloatReactiveProperty BlueEnergy { get { return blueEnergy; } }
    public float MaxEnergy { get { return maxEnergy; } }
    public IObservable<bool> CanSkill { get { return canSkill; } }

    
    ElementType castingType;

    Animator anim;

    PlayerAttribute attri;
    float lastCheck;

    void Awake()
    {
        mapping.Add(ElementType.ELEMENT_TYPE_RED,  redEnergy);
        mapping.Add(ElementType.ELEMENT_TYPE_GREEN, greenEnergy);
        mapping.Add(ElementType.ELEMENT_TYPE_BLUE, blueEnergy);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        attri = GetComponent<PlayerAttribute>();
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        if (Time.time - lastCheck <= costCheckTime)
        {
            return;
        }

        lastCheck += costCheckTime;

        Skill now = skills.SingleOrDefault(s => s.Type == castingType);
        if (now && now.IsUsing() && !now.CanSkill())
        {
            now.SkillEnd();
        }

        CheckEnergy();
    }

    void CheckEnergy()
    {
        canSkill.Value = false;
        if (attri.Type != ElementType.ELEMENT_TYPE_NONE)
        {
            mapping.ToObservable().Subscribe(p =>
            {
                if (p.Value.Value > baseCost)
                {
                    p.Value.Value -= baseCost;
                }
                else
                {
                    p.Value.Value = 0;
                }
            });
        }

        Skill now = skills.SingleOrDefault(s => s.Type == castingType);
        if (now && now.IsUsing())
        {
            Cost(now);
        }

        CheckState();
    }

    public void UseSkill()
    {
        Skill old = skills.SingleOrDefault(s => s.Type == castingType);
        if (old && old.Activated() && castingType == attri.Type)
        {
            old.SkillEnd();
        }
        else
        {
            Skill now = skills.SingleOrDefault(s => s.Type == attri.Type);
            if (now && now.CanSkill())
            {
                castingType = now.Type;
                anim.SetTrigger("Skill");
            }
        }
    }

    void DoSkill()
    {
        Skill now = skills.SingleOrDefault(s => s.Type == castingType);
        if (now && now.UseSkill())
        {
            Cost(now);
            CheckState();
        }
    }

    public void Charge(ElementType ele, float power)
    {
        if (mapping.ContainsKey(ele))
        {
            mapping[ele].Value = maxEnergy - mapping[ele].Value > power ? mapping[ele].Value + power : maxEnergy;

            CheckState();
        }
    }

    void Cost(Skill s)
    {
        if (s.RedCost > 0)
        {
            redEnergy.Value -= s.RedCost;
        }

        if (s.GreenCost > 0)
        {
            greenEnergy.Value -= s.GreenCost;
        }

        if (s.BlueCost > 0)
        {
            blueEnergy.Value -= s.BlueCost;
        }
    }

    void CheckState()
    {
        attri.AttributeChange(redEnergy.Value > 0, ElementType.ELEMENT_TYPE_RED);
        attri.AttributeChange(greenEnergy.Value > 0, ElementType.ELEMENT_TYPE_GREEN);
        attri.AttributeChange(blueEnergy.Value > 0, ElementType.ELEMENT_TYPE_BLUE);

        Skill now = skills.SingleOrDefault(s => s.Type == attri.Type);
        if (now)
        {
            canSkill.Value = now.Activated() || now.CanSkill();
        }
    }
}
