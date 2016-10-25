using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyBattle : MonoBehaviour {
    public UnitAction OnDie;
    public bool CanSlash { get { return anim.GetCurrentAnimatorStateInfo(0).fullPathHash == breakHash; } }
    public float SlashTime = 3;
    public GameObject DeadAction;
    public Vector3 DeadEffectOffset;

    EnemyMove movement;

    int idleHash;
    int breakHash;
    Animator anim;

    //Start change to Awake, because Instantiate not call Start but Awake
    void Awake()
    {
        movement = GetComponent<EnemyMove>();

        idleHash = Animator.StringToHash("EnemyBase.Idle");
        breakHash = Animator.StringToHash("EnemyBase.Break");
        anim = GetComponent<Animator>();
    }

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

    public bool Attacked(Attack attack)
    {
        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && CanSlash)
        {
            OnDie(gameObject);
            return true;
        }
        else
        {
            anim.SetTrigger("Break");
        }

        return false;
    }
}