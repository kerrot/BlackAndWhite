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
    public int gatherCount;             //amount to form ball
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

    public bool Formed { get { return current >= gatherCount; } }
    int current = 0;

    Rigidbody rd;
    int floorLayer;
    float radis;

    System.IDisposable groundSubject;

    ParticleSystem.MainModule mod;

    const float GROW_SIZE_PER_PEACE = 0.1f;
    const int POWER_PER_PEACE = 10;

    void Start()
    {
        floorLayer = LayerMask.NameToLayer("Floor");
        mod = effect.main;

        // init color
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

        if (gatherCount > 0)
        {
            AudioHelper.PlaySE(gameObject, growSE);
        }
    }

    // drop on ground
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

    // gather peace, to form ball, or grow
    public void Gather()
    {
        ++current;
        ParticleSystem.MinMaxCurve size = mod.startSize;
        if (size.constantMax < 1f)
        {
            size.constantMax += GROW_SIZE_PER_PEACE;
        }
        mod.startSize = size;

        if (Formed)
        {
            effectOn.gameObject.SetActive(true);
            lightOn.gameObject.SetActive(true);
            effect.gameObject.GetComponent<Animator>().enabled = true;
            power += POWER_PER_PEACE;
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
        if (skill && !skill.gameObject.GetComponent<PlayerSlash>().IsSlashing)
        {
            skill.Charge(Type, power);
            Destroy(gameObject);
        }
    }
}
