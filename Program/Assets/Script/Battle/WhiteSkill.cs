using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class WhiteSkill : Skill
{
    [SerializeField]
    private MeshRenderer aho;
    [SerializeField]
    private Material lightMat;
    [SerializeField]
    private ParticleSystem flash;
    [SerializeField]
    private GameObject trail;
    [SerializeField]
    private AudioClip flashSE;
    [SerializeField]
    private float period;

    Material original;
    ParticleSystem ps;

    bool flashEffect;
    float flashStart;
    GameSystem system;
    PlayerTime PT;
    Animator anim;

    void Awake()
    {
        original = aho.material;
        system = GameObject.FindObjectOfType<GameSystem>();
        PT = GameObject.FindObjectOfType<PlayerTime>();
        anim = PT.gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    public override bool Activated()
    {
        return trail.activeSelf;
    }

    public override bool UseSkill()
    {
        if (!flashEffect && system)
        {
            system.GamePause();
            system.RTM();
            flash.Play();
            AudioHelper.PlaySE(gameObject, flashSE);
            flashStart = Time.unscaledTime;
            flashEffect = true;

            anim.enabled = false;

            return true;
        }

        return false;
    }

    public override bool CanSkill()
    {
        return !flashEffect && CheckEnergy() && !trail.activeSelf;
    }

    void StartEffect()
    {
        aho.material = lightMat;
        trail.SetActive(true);

        if (PT)
        {
            PT.SlowMotion(0.2f, 0.8f);
        }

        Observable.Timer(System.TimeSpan.FromSeconds(period)).Subscribe(_ =>
        {
            aho.material = original;
            trail.SetActive(false);

            if (PT)
            {
                PT.SlowMotion(1f, 1f);
            }
        });
    }

    void UniRxUpdate()
    {
        if (flashEffect)
        {
            flash.Simulate(Time.unscaledDeltaTime, true, false);
            if (Time.unscaledTime - flashStart > flash.main.duration)
            {
                flashEffect = false;
                flash.Stop();
                if (system)
                {
                    system.GameResume();
                }
                anim.enabled = true;

                StartEffect();
            }
        }
    }
}
