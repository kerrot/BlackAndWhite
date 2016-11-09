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
        Collider[] cs = Physics.OverlapSphere(range.gameObject.transform.position, range.radius);
        cs.ToList().ForEach(c =>
        {
            PlayerBattle player = c.gameObject.GetComponent<PlayerBattle>();
            if (player && player.enabled)
            {
                player.Attacked(GetComponent<EnemyBattle>(), new Attack() { Strength = attackPower, Element = ElementType.ELEMENT_TYPE_NONE, Type = AttackType.ATTACK_TYPE_NORMAL });
                return;
            }
        });
    }
}
