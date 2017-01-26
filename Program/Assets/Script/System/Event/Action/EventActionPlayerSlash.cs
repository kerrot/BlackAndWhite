using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionPlayerSlash : EventAction
{
    public override void Launch()
    {
        PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
        if (player)
        {
            player.Slash();
        }
    }
}
