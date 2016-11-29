using UniRx;
using UnityEngine;
using System.Collections;

public class MagentaAura : AuraBattle {

    protected override void AuraStart()
    {
        AuraEffect(true);
        PlayerBattle battle = GameObject.FindObjectOfType<PlayerBattle>();
        if (battle)
        {
            battle.OnAttack.Subscribe(u => DoDisappear()).AddTo(this);
        }
    }

    protected override void AuraDisappear()
    {
        AuraEffect(false);
        gameObject.SetActive(false);
    }

    protected override void AuraRecover()
    {
        AuraEffect(true);
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
