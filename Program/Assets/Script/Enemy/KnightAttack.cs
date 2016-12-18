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
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private GameObject spawnObject;
    [SerializeField]
    private float spawnNum;
    [SerializeField]
    private GameObject magicObject;
    [SerializeField]
    private float magicPeriodMin;
    [SerializeField]
    private float magicPeriodMax;

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
        if (!coll.enabled)
        {
            return;
        }

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
                AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                if (info.fullPathHash == idleHash && Vector3.Distance(player.transform.position, transform.position) <= movement.StopRadius)
                {
                    movement.FaceTarget(player.transform.position);
                    anim.SetTrigger("Attack");
                }
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
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(shift, out navHit, 1.0f, NavMesh.AllAreas))
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
        Instantiate(magicObject, transform.position, transform.rotation);
    }
}
