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
        this.OnTriggerStayAsObservable().Subscribe(o => UniRxTriggerStay(o));
    }

    void UniRxTriggerStay(Collider other)
    {
        EnemyBattle enemy = other.GetComponent<EnemyBattle>();
        if (enemy && enemy.GetComponent<UnitMove>().CanMove 
                    && enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength)))
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
