using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For detecting red aura attacks player
public class RedEnemySpecial : MonoBehaviour {

    [SerializeField]
    private RoundDamageAura aura;

    static private Subject<Unit> burnSubject = new Subject<Unit>();
    static public IObservable<Unit> OnBurn { get { return burnSubject; } }

    void Start ()
    {
        if (aura)
        {
            aura.OnDamage.Subscribe(_ => burnSubject.OnNext(Unit.Default)).AddTo(this);
        }
	}
}
