using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionPlayerRevive : EventAction
{
    public override void Launch()
    {
        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player)
        {
            player.Revive();
        }
    }
}
