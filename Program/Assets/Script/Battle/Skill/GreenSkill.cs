using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

// player skill, stop enemy movement
public class GreenSkill : UnitBattle {

    [SerializeField]
    private GameObject GreenTrap;   //Trapped effect
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
            // Follow victom
            debuff.transform.parent = GameObject.FindObjectOfType<EnemyManager>().transform;
        }
    }

    public void End()
    {
        Destroy(gameObject);
    }
}
