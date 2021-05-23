using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject attackRange;  // range which enemy can attack
    [SerializeField]
    private float attackPower;      // damage
    [SerializeField]
    private float attackForce;      // physics
    [SerializeField]
    private AudioClip attackSE;

    int idleHash;
    Animator anim;
    EnemyMove movement;
    PlayerBattle player;
    SphereCollider range;

    // Use this for initialization
    void Awake()
    {
        idleHash = Animator.StringToHash("EnemyBase.Idle");
        anim = GetComponent<Animator>();

        movement = GetComponent<EnemyMove>();
        range = attackRange.GetComponent<SphereCollider>();
        player = GameObject.FindObjectOfType<PlayerBattle>();
    }

    // Update is called once per frame
    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        // if player can see player and in the attack range
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

    void Attack()
    {
        EnemyBattle battle = GetComponent<EnemyBattle>();

        AudioHelper.PlaySE(gameObject, attackSE);

        Collider[] cs = Physics.OverlapSphere(range.gameObject.transform.position, range.radius);
        cs.ToList().ForEach(c =>
        {
            PlayerBattle player = c.gameObject.GetComponent<PlayerBattle>();
            if (player && player.enabled)
            {
                player.Attacked(battle, battle.CreateAttack(AttackType.ATTACK_TYPE_NORMAL, attackPower, attackForce));
                return;
            }
        });
    }
}
