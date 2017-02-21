using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffect : MonoBehaviour
{
    [SerializeField]
    private EnemyBattle battle;
    [SerializeField]
    private ShakeEffect shake;

    // Use this for initialization
    void Start ()
    {
        battle.OnAttacked.Subscribe(_ => shake.enabled = true).AddTo(this);	
	}
	
}
