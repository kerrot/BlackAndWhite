using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject startAttack;
    [SerializeField]
    private GameObject attackRange;
    [SerializeField]
    private float attackPower;
    [SerializeField]
    private float attackForce;

    int idleHash;
    Animator anim;
    EnemyMove movement;

    SphereCollider range;

    // Use this for initialization
    void Awake()
    {
        idleHash = Animator.StringToHash("EnemyBase.Idle");
        anim = GetComponent<Animator>();

        movement = GetComponent<EnemyMove>();
        range = attackRange.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player && player.enabled)
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

        Collider[] cs = Physics.OverlapSphere(range.gameObject.transform.position, range.radius);
        cs.ToList().ForEach(c =>
        {
            PlayerBattle player = c.gameObject.GetComponent<PlayerBattle>();
            if (player && player.enabled)
            {
                player.Attacked(GetComponent<EnemyBattle>(), battle.CreateAttack(AttackType.ATTACK_TYPE_NORMAL, attackPower, attackForce));
                return;
            }
        });
    }
}
