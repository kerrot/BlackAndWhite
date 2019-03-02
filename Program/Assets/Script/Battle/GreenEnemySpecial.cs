using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For detecting green aura attacked
public class GreenEnemySpecial : MonoBehaviour {

    [SerializeField]
    private BlockAttackAura aura;

    static private Subject<Unit> blockSubject = new Subject<Unit>();
    static public IObservable<Unit> OnBlock { get { return blockSubject; } }

    void Start ()
    {
        if (aura)
        {
            aura.OnBlock.Subscribe(_ => blockSubject.OnNext(Unit.Default)).AddTo(this);
        }
	}
}
