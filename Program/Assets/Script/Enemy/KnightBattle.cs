using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class KnightBattle : UnitBattle {

    [SerializeField]
    private float HPMax;
    [SerializeField]
    private float deadTime;
    [SerializeField]
    private float showHPTime;
    [SerializeField]
    private Transform HPUICenter;
    [SerializeField]
    private GameObject wanderEffect;

    private Subject<GameObject> dieSubject = new Subject<GameObject>();

    public IObservable<GameObject> OnDie { get { return dieSubject; } }

    HPBarUI hpUI;
    float showHPStart;

    Animator anim;

    float deadStart;

    Collider coll;

    int wanderHash;
    int damageHash;

    bool dead;
    PlayerBattle player;
    Attribute attr;

    void Awake()
    {
        attr = GetComponent<Attribute>();

        player = GameObject.FindObjectOfType<PlayerBattle>();
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        wanderHash = Animator.StringToHash("EnemyBase.Wander");


        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
        this.OnAnimatorMoveAsObservable().Subscribe(_ => UniRxAnimatorMove());
    }

    void UniRxAnimatorMove()
    {
        transform.position = anim.rootPosition;
    }

    void UniRxUpdate()
    {
        

        if (player)
        {
            anim.SetBool("Wander", player.Missing);
            wanderEffect.SetActive(player.Missing);
        }
    }

    public override bool Attacked(UnitBattle unit, Attack attack)
    {
        

        return false;
    }

    public void RecoverFromDamage()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash == damageHash)
        {
            anim.SetTrigger("DamageEnd");
        }
    }

    void Die(Attack attack)
    {
        
        

        dead = true;

        dieSubject.OnNext(gameObject);
    }

    void UniRxOnDestroy()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
