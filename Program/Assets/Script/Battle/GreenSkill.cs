using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class GreenSkill : UnitBattle {

    [SerializeField]
    private GameObject GreenTrap;
    [SerializeField]
    private float strength;

    void Start()
    {
        this.OnTriggerEnterAsObservable().Subscribe(o => UniRxTriggerEnter(o));
    }

    void UniRxTriggerEnter(Collider other)
    {
        EnemyBattle enemy = other.GetComponent<EnemyBattle>();
        if (enemy && enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength)))
        {
            GameObject debuff = Instantiate(GreenTrap, enemy.gameObject.transform.position, Quaternion.identity) as GameObject;
            debuff.GetComponent<StopMove>().victom = enemy.GetComponent<EnemyMove>();
        }
    }

    public void End()
    {
        Destroy(gameObject);
    }
}
