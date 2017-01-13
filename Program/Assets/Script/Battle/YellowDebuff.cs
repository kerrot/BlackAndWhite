using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using System.Collections;

public class YellowDebuff : AuraBattle
{
    public EnemyBattle vistom { get; set; }

    PlayerAttribute attri;

    protected override void AuraStart()
    {
        DoRecover();
        attri = GameObject.FindObjectOfType<PlayerAttribute>();
    }

    protected override void AuraDisappear()
    {
        if (vistom)
        {
            vistom.RecoverFromDamage();
        }
        Destroy(gameObject);
    }

    protected override void AuraUpdate()
    {
        if (attri && attri.Type != element)
        {
            AuraDisappear();
        }
    }

    public void End()
    {
        PlayerMove player = GameObject.FindObjectOfType<PlayerMove>();
        if (player)
        {
            player.transform.position = transform.position;
            player.MoveStop();
        }

        AuraDisappear();
    }
}
