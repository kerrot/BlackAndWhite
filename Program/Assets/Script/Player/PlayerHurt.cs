using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour {
    [SerializeField]
    private PlayerBattle battle;
    [SerializeField]
    private float scale = 50;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float danger = 0.5f;

    private Subject<bool> dangerSubject = new Subject<bool>();

    public IObservable<bool> OnDanger { get { return dangerSubject; } }

    RunTimeUIGenerator ui;

    void Start ()
    {
        ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();

        battle.OnAttacked.Subscribe(u => Attacked(u)).AddTo(this);
        battle.HPRate.Subscribe(v => 
        {
            anim.SetBool("danger", v < danger);
            dangerSubject.OnNext(v < danger);
        });
    }

    void Attacked(UnitBattle unit)
    {
        GameObject obj = ui.CreateHurtUI();

        Vector3 direction = unit.transform.position - battle.transform.position;
        float angle = ((direction.x > 0) ? -1 : 1 ) * Vector3.Angle(direction, Vector3.forward);
        obj.transform.Rotate(0, 0, angle);

        Vector3 offset = direction.normalized * scale;

        obj.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(offset.x, offset.z, 0);
    }
}
