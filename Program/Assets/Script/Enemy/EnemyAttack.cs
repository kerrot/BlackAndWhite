using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject attakRange;
    [SerializeField]
    private float attakAngle;

    int idleHash;
    Animator anim;
    EnemyMove movement;

    float radius;

    // Use this for initialization
    void Awake()
    {
        idleHash = Animator.StringToHash("EnemyBase.Idle");
        anim = GetComponent<Animator>();

        movement = GetComponent<EnemyMove>();
        radius = attakRange.transform.localScale.x / 2;
    }

    // Update is called once per frame
    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        PlayerMove player = GameObject.FindObjectOfType<PlayerMove>();
        if (player)
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.fullPathHash == idleHash && Vector3.Distance(player.transform.position, transform.position) <= movement.StopRadius)
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    void Attack()
    {
        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player)
        {
            Vector3 direction = player.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            if (direction.magnitude <= radius && angle <= attakAngle)
            {
                player.Attacked(new Attack() { Element = ElementType.ELEMENT_TYPE_NONE, Type = AttackType.ATTACK_TYPE_NORMAL });
            }
        }
    }
}
