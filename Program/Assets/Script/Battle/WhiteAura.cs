using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class WhiteAura : AuraBattle {
    [SerializeField]
    private MeshRenderer aho;
    [SerializeField]
    private Material lightMat;
    [SerializeField]
    private ParticleSystem flash;
    [SerializeField]
    private GameObject trail;

    Material original;
    ParticleSystem ps;

    bool flashEffect;
    float flashStart;

    protected override void AuraStart()
    {
        original = aho.material;
    }

    protected override void AuraDisappear()
    {
        aho.material = original;
        trail.SetActive(false);

        PlayerTime PT = GameObject.FindObjectOfType<PlayerTime>();
        if (PT)
        {
            PT.SlowMotion(1f, 1f);
        }
    }

    protected override void AuraRecover()
    {
        aho.material = lightMat;
        trail.SetActive(true);

        PlayerTime PT = GameObject.FindObjectOfType<PlayerTime>();
        if (PT)
        {
            PT.SlowMotion(0.5f, 1f);
        }
    }

    protected override void AuraUpdate()
    {
        if (flashEffect)
        {
            flash.Simulate(Time.unscaledDeltaTime, true, false);
            if (Time.unscaledTime - flashStart > flash.duration)
            {
                flashEffect = false;
                flash.Stop();
                GameSystem system = GameObject.FindObjectOfType<GameSystem>();
                if (system)
                {
                    system.GameResume();
                }
                DoRecover();
            }
        }
    }

    public void Launch()
    {
        if (IsAura)
        {
            return;
        }

        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GamePause();
            system.RTM();
            flash.Play();
            flashStart = Time.unscaledTime;
            flashEffect = true;
        }
    }
}
