using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

// boss attack ai
public class KnightAttack : MonoBehaviour {
    [SerializeField]
    private GameObject weapon;          // for collider
    [SerializeField]
    private float attackPower;          // damage
    [SerializeField]
    private float attackForce;          // physics
    [SerializeField]
    private float spawnTime;            // summon minions period
    [SerializeField]
    private GameObject spawnObject;     // minion to summon
    [SerializeField]
    private float spawnNum;             // minion amount
    [SerializeField]
    private GameObject magicObject;     // boss skill
    [SerializeField]
    private float magicPeriodMin;
    [SerializeField]
    private float magicPeriodMax;
    [SerializeField]
    private AudioClip attackSE;

    Collider weaponCollider;
    Animator anim;
    UnitBattle battle;
    PlayerBattle player;
    EnemyMove movement;
    int idleHash;
    int attackHash;
    System.IDisposable attackDis;

    float spawnStart;
    int spawnCount;

    float magicTime;
    bool magicRandom = true;

    EnemyManager manager;
    Collider coll;

    void Start()
    {
        if (weapon)
        {
            weaponCollider = weapon.GetComponent<Collider>();
        }

        idleHash = Animator.StringToHash("EnemyBase.Idle");
        attackHash = Animator.StringToHash("EnemyBase.Attack");
        anim = GetComponent<Animator>();
        battle = GetComponent<UnitBattle>();
        movement = GetComponent<EnemyMove>();
        player = GameObject.FindObjectOfType<PlayerBattle>();

        manager = GameObject.FindObjectOfType<EnemyManager>();

        coll = GetComponent<Collider>();

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        // prevent attack when not attack
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash != attackHash)
        {
            weaponCollider.enabled = false;
            if (attackDis != null)
            {
                attackDis.Dispose();
            }
        }

        if (!coll.enabled)
        {
            return;
        }

        // priority:  summon > magic > attack
        if (spawnObject && Time.time - spawnStart > spawnTime)
        {
            spawnStart = Time.time;
            if (spawnCount < spawnNum)
            {
                anim.SetTrigger("Summon");
                anim.SetBool("Attack", false);
                return;
            }
        }

        if (player && !player.Missing)
        {
            if (magicObject && Time.time > magicTime)
            {
                // random the time to use skill
                if (magicRandom)
                {
                    magicTime = Time.time + Random.Range(magicPeriodMin, magicPeriodMax);
                    magicRandom = false;
                }
                else
                {
                    movement.FaceTarget(player.transform.position);
                    anim.SetTrigger("Magic");
                    anim.SetBool("Attack", false);
                    magicRandom = true;
                }
            }
            else
            {
                // check range. prevent wrong anim state
                AnimatorStateInfo idleInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (idleInfo.fullPathHash == idleHash && Vector3.Distance(player.transform.position, transform.position) <= movement.StopRadius
                                                      && !anim.GetBool("Attack")  )
                {
                    anim.SetTrigger("Attack");
                }
            }
        }
    }

    // call by animation
    void AttackInit()
    {
        movement.FaceTarget(player.transform.position);
    }

    // call by animation
    void AttackStart()
    {
        AudioHelper.PlaySE(gameObject, attackSE);
        weaponCollider.enabled = true;
        attackDis = this.OnTriggerEnterAsObservable().Subscribe(o => UniRxTriggerEnter(o));
    }

    // call by animation
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

    void Summon()
    {
        if (!manager)
        {
            return;
        }

        anim.SetBool("Summon", false);

        while (spawnCount < spawnNum)
        {
            Vector2 offset = Random.insideUnitCircle;
            Vector3 shift = new Vector3(offset.x, 0, offset.y) * movement.StopRadius + transform.position;
            // check spawn in lacation that can be reached
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(shift, out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                shift = navHit.position;
            }
            else
            {
                shift = transform.position;
            }

            GameObject obj = manager.CreateEnemy(spawnObject, shift, Quaternion.identity);
            obj.OnDestroyAsObservable().Subscribe(_ => --spawnCount).AddTo(this);
            ++spawnCount;
        }
    }

    void Magic()
    {
        GameObject obj = Instantiate(magicObject, transform.position, transform.rotation);
        obj.transform.parent = manager.transform;
    }
}
