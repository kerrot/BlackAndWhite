using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class BlueSpellEffect : AuraBattle {

    [SerializeField]
    private float strength;

    ParticleSystem.EmissionModule em;

    protected override void AuraStart()
    {
        em = GetComponent<ParticleSystem>().emission;
        element = ElementType.ELEMENT_TYPE_BLUE;
        DoRecover();

        this.OnTriggerEnterAsObservable().Subscribe(o => UniRxOnTriggerEnter(o));
    }

    protected override void AuraDisappear()
    {
        GetComponent<Collider>().enabled = false;
        em.enabled = false;
        Destroy(gameObject, 1f);
    }

    void UniRxOnTriggerEnter(Collider other)
    {
        EnemyBattle enemy = other.gameObject.GetComponent<EnemyBattle>();
        if (enemy)
        {
            enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength));
        }

        //PlayerBattle player = other.gameObject.GetComponent<PlayerBattle>();
        //if (player)
        //{

        //}
    }
}
