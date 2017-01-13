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
    [SerializeField]
    private AudioClip growSE;
    [SerializeField]
    private AudioClip formSE;

    static private Subject<EnergyBall> formSubject = new Subject<EnergyBall>();
    static public IObservable<EnergyBall> OnForm { get { return formSubject; } }

    static private Subject<EnergyBall> newSubject = new Subject<EnergyBall>();
    static public IObservable<EnergyBall> OnNew { get { return newSubject; } }

    public bool Formed { get { return current == gatherCount; } }
    int current = 0;

    Rigidbody rd;
    int floorLayer;
    float radis;

    System.IDisposable groundSubject;

    ParticleSystem.MainModule mod;

    void Start()
    {
        floorLayer = LayerMask.NameToLayer("Floor");
        mod = effect.main;

        switch (Type)
        {
            case ElementType.ELEMENT_TYPE_RED:
                mod.startColor = red;
                break;
            case ElementType.ELEMENT_TYPE_GREEN:
                mod.startColor = green;
                break;
            case ElementType.ELEMENT_TYPE_BLUE:
                mod.startColor = blue;
                break;
        }

        Color tmp = Attribute.GetColor(Type, 1.0f);
        lightOn.color = tmp;
        radis = GetComponent<SphereCollider>().radius;

        rd = GetComponent<Rigidbody>();

        if (Formed)
        {
            this.OnTriggerStayAsObservable().Subscribe(o => PlayerCharge(o));
        }
        else
        {
            groundSubject = this.OnTriggerEnterAsObservable().Subscribe(o => OnGround(o));
        }

        AudioHelper.PlaySE(gameObject, growSE);
    }

    void OnGround(Collider other)
    {
        if (other.gameObject.layer == floorLayer)
        {
            rd.useGravity = false;
            rd.velocity = Vector3.zero;

            transform.position = new Vector3(transform.position.x, radis, transform.position.z);
            groundSubject.Dispose();

            newSubject.OnNext(this);
        }
    }

    public void Gather()
    {
        if (Formed)
        {
            return;
        }

        ++current;
        ParticleSystem.MinMaxCurve size = mod.startSize;
        size.constantMax += 0.1f;
        mod.startSize = size;

        if (Formed)
        {
            effectOn.gameObject.SetActive(true);
            lightOn.gameObject.SetActive(true);
            effect.gameObject.GetComponent<Animator>().enabled = true;

            AudioHelper.PlaySE(gameObject, formSE);

            formSubject.OnNext(this);
            this.OnTriggerStayAsObservable().Subscribe(o => PlayerCharge(o));
        }
        else
        {
            AudioHelper.PlaySE(gameObject, growSE);
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
