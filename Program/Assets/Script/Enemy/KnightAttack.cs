using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class KnightAttack : MonoBehaviour {
    [SerializeField]
    private GameObject weapon;
    [SerializeField]
    private float attackPower;
    [SerializeField]
    private float attackForce;

    Collider weaponCollider;
    Animator anim;
    UnitBattle battle;
    PlayerBattle player;
    EnemyMove movement;
    int idleHash;
    int attackHash;
    System.IDisposable attackDis;

    void Start()
    {
        if (weapon)
        {
            weaponCollider = weapon.GetComponent<Collider>();
        }

        idleHash = Animator.StringToHash("EnemyBase.Idle");
        anim = GetComponent<Animator>();
        battle = GetComponent<UnitBattle>();
        movement = GetComponent<EnemyMove>();
        player = GameObject.FindObjectOfType<PlayerBattle>();

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        if (player && !player.Missing)
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.fullPathHash == idleHash && Vector3.Distance(player.transform.position, transform.position) <= movement.StopRadius)
            {
                movement.FaceTarget(player.transform.position);
                anim.SetTrigger("Attack");
                
            }
        }
    }

    void AttackStart()
    {
        weaponCollider.enabled = true;
        attackDis = this.OnTriggerEnterAsObservable().Subscribe(o => UniRxTriggerEnter(o));
    }

    void AttackEnd()
    {
        weaponCollider.enabled = false;
        attackDis.Dispose();
    }

    void UniRxTriggerEnter(Collider other)
    {
        PlayerBattle player = other.gameObject.GetComponent<PlayerBattle>();
        if (player && player.enabled)
        {
            player.Attacked(battle, battle.CreateAttack(AttackType.ATTACK_TYPE_NORMAL, attackPower, attackForce));
        }
    }
}
