using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnergyBall : EnergyBase
{
    [SerializeField]
    private Color blue;
    [SerializeField]
    private Color red;
    [SerializeField]
    private Color green;
    [SerializeField]
    private int gatherCount;
    [SerializeField]
    private ParticleSystem effect;
    [SerializeField]
    private ParticleSystem effectOn;
    [SerializeField]
    private Light lightOn;
    [SerializeField]
    private float power;

    public bool Formed { get { return current == gatherCount; } }
    int current = 0;

    Rigidbody rd;
    System.IDisposable dis;

    void Start()
    {
        EnergyPeace.NewBorn.OnNext(this);
        switch (Type)
        {
            case ElementType.ELEMENT_TYPE_RED:
                effect.startColor = red;
                break;
            case ElementType.ELEMENT_TYPE_GREEN:
                effect.startColor = green;
                break;
            case ElementType.ELEMENT_TYPE_BLUE:
                effect.startColor = blue;
                break;
        }

        Color tmp = Attribute.GetColor(Type, 1.0f);
        effectOn.startColor = tmp;
        lightOn.color = tmp;

        rd = GetComponent<Rigidbody>();
        rd.velocity = Vector3.up;

        dis = this.UpdateAsObservable().Subscribe(_ => OnGround());
    }

    void OnGround()
    {
        if (rd.velocity == Vector3.zero)
        {
            GetComponent<Collider>().isTrigger = true;
            rd.useGravity = false;
            dis.Dispose();
        }
    }

    public void Gather()
    {
        if (Formed)
        {
            return;
        }

        ++current;
        effect.startSize += 0.1f;
        if (Formed)
        {
            effectOn.gameObject.SetActive(true);
            lightOn.gameObject.SetActive(true);
            this.OnTriggerEnterAsObservable().Subscribe(o => UniRxTriggerEnter(o));
        }
    }

    void UniRxTriggerEnter(Collider other)
    {
        PlayerSkill skill = other.gameObject.GetComponent<PlayerSkill>();
        if (skill)
        {
            skill.Charge(Type, power);
            Destroy(gameObject);
        }
    }
}
