using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using System.Collections;

public class YellowDebuff : AuraBattle
{
    public EnemyBattle vistom { get; set; }

    protected override void AuraStart()
    {
        DoRecover();
    }

    protected override void AuraDisappear()
    {
        if (vistom)
        {
            vistom.RecoverFromDamage();
        }
        Destroy(gameObject);
    }

    public void End()
    {
        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player)
        {
            player.transform.position = transform.position;
        }

        AuraDisappear();
    }
}
