using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionPlayerAnimation : EventAction
{
    [SerializeField]
    private string stateName;

    public override void Launch()
    {
        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player)
        {
            player.gameObject.GetComponent<Animator>().Play(stateName);
        }
    }
}
