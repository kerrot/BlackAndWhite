﻿using UnityEngine;
using System.Collections;

//automically teleport to player position if in [range], used by boss skill
public class TracePlayerObject : MonoBehaviour
{
    [SerializeField]
    private float range;

	void Start ()
    {
        PlayerBattle battle = GameObject.FindObjectOfType<PlayerBattle>();
	    if (battle && !battle.Missing)
        {
            Vector3 diff = battle.transform.position - transform.position;

            if (diff.magnitude < range)
            {
                transform.position = battle.transform.position;
            }
            else
            {
                transform.position += diff.normalized * range;
            }
        }
	}
}
