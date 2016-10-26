using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyBattle : MonoBehaviour {

    [SerializeField]
    private float barrierStrength;
    [SerializeField]
    private float recoverSpeed;

    public UnitAction OnDie;
    
    public GameObject DeadAction;
    public Vector3 DeadEffectOffset;

    EnemyMove movement;
    EnemySlash slash;

    int idleHash;
    Animator anim;
    
    float currentBarrier;



    //Start change to Awake, because Instantiate not call Start but Awake
    void Awake()
    {
        movement = GetComponent<EnemyMove>();
        slash = GetComponent<EnemySlash>();

        idleHash = Animator.StringToHash("EnemyBase.Idle");
        anim = GetComponent<Animator>();
        
        currentBarrier = barrierStrength;
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

        currentBarrier += recoverSpeed * Time.deltaTime;
        if (currentBarrier > barrierStrength)
        {
            currentBarrier = barrierStrength;
        }
    }

    public bool Attacked(Attack attack)
    {
        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && slash.CanSlash)
        {
            OnDie(gameObject);
            return true;
        }
        else
        {
            currentBarrier -= attack.Strength;
            if (currentBarrier > 0)
            {
                anim.SetTrigger("Hitted");
            }
            else
            {
                slash.TriggerSlash();
            }
        }

        return false;
    }
}