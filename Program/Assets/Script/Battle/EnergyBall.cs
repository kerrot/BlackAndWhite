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

    static private Subject<EnergyBall> formSubject = new Subject<EnergyBall>();
    static public IObservable<EnergyBall> OnForm { get { return formSubject; } }

    public bool Formed { get { return current == gatherCount; } }
    int current = 0;

    Rigidbody rd;
    int floorLayer;
    float radis;

    System.IDisposable groundSubject;

    void Start()
    {
        floorLayer = LayerMask.NameToLayer("Floor");

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
        radis = GetComponent<SphereCollider>().radius;

        rd = GetComponent<Rigidbody>();

        groundSubject = this.OnTriggerEnterAsObservable().Subscribe(o => OnGround(o));
    }

    void OnGround(Collider other)
    {
        if (other.gameObject.layer == floorLayer)
        {
            rd.useGravity = false;
            rd.velocity = Vector3.zero;

            transform.position = new Vector3(transform.position.x, radis, transform.position.z);
            groundSubject.Dispose();
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

            formSubject.OnNext(this);
            this.OnTriggerStayAsObservable().Subscribe(o => PlayerCharge(o));
        }
    }

    void PlayerCharge(Collider other)
    {
        PlayerSkill skill = other.gameObject.GetComponent<PlayerSkill>();
        if (skill)
        {
            skill.Charge(Type, power);
            Destroy(gameObject);
        }
    }
}
