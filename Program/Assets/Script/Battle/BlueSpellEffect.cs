using UnityEngine;
using System.Collections;

public class BlueSpellEffect : AuraBattle {

    [SerializeField]
    private float lastTime;
    [SerializeField]
    private float strength;

    ParticleSystem.EmissionModule em;

    void Start ()
    {
        em = GetComponent<ParticleSystem>().emission;
    }

    public void End()
    {
        GetComponent<Collider>().enabled = false;
        em.enabled = false;
        Destroy(gameObject, 1f);
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyBattle enemy = other.gameObject.GetComponent<EnemyBattle>();
        if (enemy)
        {
            enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength));
        }

        PlayerBattle player = other.gameObject.GetComponent<PlayerBattle>();
        if (player)
        {

        }
    }
}
