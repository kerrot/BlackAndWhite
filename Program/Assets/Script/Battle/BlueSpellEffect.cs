using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class BlueSpellEffect : AuraBattle {

    [SerializeField]
    private float lastTime;
    [SerializeField]
    private float strength;

    ParticleSystem.EmissionModule em;

    float startTime;

    void Start ()
    {
        em = GetComponent<ParticleSystem>().emission;

        Attribute attr = GetComponent<Attribute>();
        if (!attr)
        {
            attr = gameObject.AddComponent<Attribute>();
        }

        attr.SetElement(ElementType.ELEMENT_TYPE_BLUE);

        startTime = Time.time;

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnTriggerEnterAsObservable().Subscribe(o => UniRxOnTriggerEnter(o));
    }

    void UniRxUpdate()
    {
        if (Time.time - startTime > lastTime)
        {
            End();
        }
    }

    public void End()
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
