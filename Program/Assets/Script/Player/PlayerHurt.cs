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
    private float danger = 0.5f;

    RunTimeUIGenerator ui;

    Animator anim;

    void Start ()
    {
        ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        anim = Camera.main.gameObject.GetComponent<Animator>();

        battle.OnAttacked.Subscribe(u => Attacked(u)).AddTo(this);
        battle.HPRate.Subscribe(v => anim.SetBool("danger", v < danger));
    }

    void Attacked(UnitBattle unit)
    {
        GameObject obj = ui.CreateHurtUI();

        Vector3 direction = unit.transform.position - battle.transform.position;
        float angle = ((direction.x > 0) ? -1 : 1 ) * Vector3.Angle(direction, Vector3.forward);
        Debug.Log(direction + " " + angle);
        obj.transform.Rotate(0, 0, angle);

        Vector3 offset = direction.normalized * scale;

        obj.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(offset.x, offset.z, 0);
    }
}
