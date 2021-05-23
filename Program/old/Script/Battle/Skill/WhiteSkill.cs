using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

//player skill, slow motion
public class WhiteSkill : Skill
{
    //for  white aura effect
    [SerializeField]
    private MeshRenderer aho;   
    [SerializeField]
    private Material lightMat;
    [SerializeField]
    private GameObject trail;

    // effect when  execute skill
    [SerializeField]
    private ParticleSystem flash;
    [SerializeField]
    private AudioClip flashSE;
    [SerializeField]
    private float period;

    [SerializeField]
    private float slowMotion = 0.2f;
    [SerializeField]
    private float playerSpeed = 0.8f;

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

    // effect when execute skill (a flash), game pause
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

    // slow motion effect
    void StartEffect()
    {
        aho.material = lightMat;
        trail.SetActive(true);

        if (PT)
        {
            PT.SlowMotion(slowMotion, playerSpeed);
        }

        Observable.Timer(System.TimeSpan.FromSeconds(period)).Subscribe(_ =>
        {
            aho.material = original;
            trail.SetActive(false);

            if (PT)
            {   // recover
                PT.SlowMotion(1f, 1f);
            }
        });
    }

    void UniRxUpdate()
    {
        if (flashEffect)
        {
            //because of game pause
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
