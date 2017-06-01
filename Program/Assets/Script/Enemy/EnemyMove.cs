using UniRx;
using UniRx.Triggers;

using UnityEngine;
using System.Collections;

// move toward player, and teleport if in certain range
public class EnemyMove : UnitMove {

    [SerializeField]
    private float stopRadius;               // distance to stop
    [SerializeField]
    private float teleportPeriodMin;
    [SerializeField]
    private float teleportPeriodMax;
    [SerializeField]
    private float teleportMaxDistance;
    [SerializeField]
    private float teleportMinDistance;
    [SerializeField]
    private GameObject warpObj;             // the mark of the position to teleport
    [SerializeField]
    private AudioClip teleportSE;

    public float StopRadius { get { return stopRadius; } }

    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;

    int moveHash;
    int idleHash;
    int attackHash;

    bool needRandom = true;
    float teleportTime;
    PlayerBattle player;

    const float MIN_ANGLE_TO_TURN = 10f;

    void Start () {
        this.UpdateAsObservable().Subscribe(__ => UniRxUpdate());
		
        anim = GetComponent<Animator>();
        player = GameObject.FindObjectOfType<PlayerBattle>();

        // move by root motion, only control the direction, base on navigation
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        moveHash = Animator.StringToHash("EnemyBase.Move");
        idleHash = Animator.StringToHash("EnemyBase.Idle");
        attackHash = Animator.StringToHash("EnemyBase.Attack");
    }

	
	void UniRxUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        anim.SetBool("Move", false);

        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player && !player.Missing && CanMove)
        {
            agent.destination = player.transform.position;

            float distance = Vector3.Distance(player.transform.position, transform.position);
            anim.SetBool("Move", distance > stopRadius);

            if (info.fullPathHash == moveHash)
            {
                FaceTarget(agent.steeringTarget);
                agent.nextPosition = transform.position;
            }

            if (distance > teleportMinDistance && distance < teleportMaxDistance)
            {
                // readom the time to teleport
                if (needRandom)
                {
                    teleportTime = Time.time + Random.Range(teleportPeriodMin, teleportPeriodMax);
                    needRandom = false;
                }
                else if (Time.time > teleportTime)
                {
                    anim.SetTrigger("TelePort");
                    needRandom = true;
                }
            }
            else
            {
                needRandom = true;
            }
        }

        if (info.fullPathHash == idleHash)
        {
            warpObj.SetActive(false);
        }
    }

    public void FaceTarget(Vector3 target)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.fullPathHash == attackHash)
        {
            return;
        }

        Vector3 offset = (target == transform.position) ? transform.forward : (target - transform.position).normalized;
        float angle = Vector3.Angle(offset, Vector3.forward) * ((offset.x > 0) ? 1 : -1);
        float diff = Vector3.Angle(offset, transform.forward);
        if (Mathf.Abs(diff) > MIN_ANGLE_TO_TURN)
        {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }

    // call by animaiton
    void StartTeleport()
    {
        if (player && !player.Missing && CanMove)
        {
            // readon the position to teleport
            Vector2 offset = Random.insideUnitCircle * stopRadius;
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(player.transform.position + new Vector3(offset.x, 0, offset.y), out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                Vector3 pos = navHit.position;
                pos.y = 0f;
                warpObj.transform.position = pos;
                
                warpObj.SetActive(true);
            }
        }
    }

    // call by animaiton
    void Teleport()
    {
        warpObj.SetActive(false);
        if (CanMove)
        {
            transform.position = warpObj.transform.position;
            AudioHelper.PlaySE(gameObject, teleportSE);
        }
    }
}
