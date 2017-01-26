using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
