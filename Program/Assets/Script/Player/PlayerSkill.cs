using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//for using skill, and energy management
public class PlayerSkill : MonoBehaviour
{
    [SerializeField]
    private float maxEnergy;
    [SerializeField]
    private float baseCost;         // need energy even not using skill
    [SerializeField]
    private float costCheckTime;    // use energy for [costCheckTime] when aura type
    [SerializeField]
    private Skill[] skills;
    [SerializeField]
    private ParticleSystem charge;  // effect when energy charged
    [SerializeField]
    private AudioClip chargeSE;

    private Subject<ElementType> chargeSubject = new Subject<ElementType>();
    private Subject<ElementType> skillSubject = new Subject<ElementType>();

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
    public IObservable<ElementType> OnCharge { get { return chargeSubject; } }
    public IObservable<ElementType> OnSkill { get { return skillSubject; } }

    // the Element Type when casting
    ElementType castingType;

    Animator anim;

    PlayerAttribute attri;
    PlayerSlash slash;
    float lastCheck;
    int skillHash;

    bool isSkilling;

    void Awake()
    {
        skillHash = Animator.StringToHash("PlayerBase.Skill");

        mapping.Add(ElementType.ELEMENT_TYPE_RED,  redEnergy);
        mapping.Add(ElementType.ELEMENT_TYPE_GREEN, greenEnergy);
        mapping.Add(ElementType.ELEMENT_TYPE_BLUE, blueEnergy);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        attri = GetComponent<PlayerAttribute>();
        slash = GetComponent<PlayerSlash>();

        InputController.OnRightMouseDown.Subscribe(v => UseSkill(EnemyManager.GetEnemyByMousePosition(v))).AddTo(this);

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash != skillHash)
        {
            isSkilling = false;
        }

        if (Time.time - lastCheck > costCheckTime)
        {
            CheckEnergy();
            lastCheck = Time.time;
        }
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

        // aura type
        Skill now = skills.SingleOrDefault(s => s.Type == castingType);
        if (now && now.IsUsing())
        {
            Cost(now);
        }

        CheckState();
    }

    public void UseSkill(GameObject obj)
    {
        if (PlayerBattle.IsDead)
        {
            return;
        }

        if (slash && slash.IsSlashing)
        {
            return;
        }

        // skill can be canceled when using aura type skill.
        // Only do the animation, unless no need
        Skill old = skills.SingleOrDefault(s => s.Type == castingType);
        if (old && old.Activated() && castingType == attri.Type)
        {
            old.SkillEnd();
        }
        else
        {
            Skill now = skills.SingleOrDefault(s => s.Type == attri.Type);
            if (now && now.CanSkill() && !isSkilling)
            {
                if (obj != null)
                {
                    Vector3 pos = obj.transform.position;
                    pos.y = 0f;
                    transform.LookAt(pos);
                }

                castingType = now.Type;

                if (now.castMotion)
                {
                    anim.SetTrigger("Skill");
                    isSkilling = true;
                }
                else
                {
                    DoSkill();
                }
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

        skillSubject.OnNext(castingType);
    }

    public void Charge(ElementType ele, float power)
    {
        if (mapping.ContainsKey(ele))
        {
            mapping[ele].Value = maxEnergy - mapping[ele].Value > power ? mapping[ele].Value + power : maxEnergy;

            CheckState();

            chargeSubject.OnNext(ele);

            GameObject obj = Instantiate(charge.gameObject);

            ParticleSystem.MainModule mod = obj.GetComponent<ParticleSystem>().main;
            mod.startColor = Attribute.GetColor(ele, 1.0f);

            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            AudioHelper.PlaySE(gameObject, chargeSE);
        }
    }

    void Cost(Skill s)
    {
        if (s.RedCost > 0)
        {
            redEnergy.Value = (redEnergy.Value > s.RedCost) ? redEnergy.Value - s.RedCost : 0;
        }

        if (s.GreenCost > 0)
        {
            greenEnergy.Value = (greenEnergy.Value > s.GreenCost) ? greenEnergy.Value - s.GreenCost : 0;
        }

        if (s.BlueCost > 0)
        {
            blueEnergy.Value = (blueEnergy.Value > s.BlueCost) ? blueEnergy.Value - s.BlueCost : 0;
        }
    }

    void CheckState()
    {
        ElementType oldType = attri.Type;

        attri.AttributeChange(redEnergy.Value > 0, ElementType.ELEMENT_TYPE_RED);
        attri.AttributeChange(greenEnergy.Value > 0, ElementType.ELEMENT_TYPE_GREEN);
        attri.AttributeChange(blueEnergy.Value > 0, ElementType.ELEMENT_TYPE_BLUE);

        Skill now = skills.SingleOrDefault(s => s.Type == attri.Type);
        if (now)
        {
            canSkill.Value = now.Activated() || now.CanSkill();
        }

        if (oldType != attri.Type)
        {
            Skill old = skills.SingleOrDefault(s => s.Type == oldType);
            if (old)
            {
                old.SkillEnd();
            }
        }
    }
}
