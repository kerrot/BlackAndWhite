using UnityEngine;
using System.Collections;

public class MagentaAura : AuraBattle {

    protected override void AuraStart()
    {
        AuraEffect(true);
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
