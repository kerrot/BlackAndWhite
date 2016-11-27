using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using System.Collections;

public class YellowDebuff : AuraBattle
{
    public EnemyBattle vistom { get; set; }

    protected override void AuraDisappear()
    {
        End();
    }

    public void End()
    {
        if (vistom)
        {
            vistom.RecoverFromDamage();
        }
        Destroy(gameObject);
    }
}
