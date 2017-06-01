using UniRx;
using UnityEngine;
using System.Collections;

public class MagentaAura : AuraBattle {
    [SerializeField]
    private AudioClip startSE;
    [SerializeField]
    private ParticleSystem startEffect;


    protected override void AuraStart()
    {
        PlayerBattle battle = GameObject.FindObjectOfType<PlayerBattle>();
        if (battle)
        {
            battle.OnAttack.Subscribe(u => DoDisappear()).AddTo(this);
        }

        DoRecover();
    }

    protected override void AuraDisappear()
    {
        AuraEffect(false);
        gameObject.SetActive(false);
    }

    protected override void AuraRecover()
    {
        AuraEffect(true);
        startEffect.Play();
        AudioHelper.PlaySE(gameObject, startSE);
    }

    void AuraEffect(bool effect)
    {
        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player)
        {
            player.Missing = effect;
            player.GetComponent<Collider>().isTrigger = effect;
        }
    }
}
