using UniRx;
using UniRx.Triggers;

using UnityEngine;
using System.Collections;

public class EnemyMove : UnitMove {

    [SerializeField]
    private float stopRadius;
    [SerializeField]
    private float teleportPeriodMin;
    [SerializeField]
    private float teleportPeriodMax;
    [SerializeField]
    private float teleportMaxDistance;
    [SerializeField]
    private float teleportMinDistance;

    public float StopRadius { get { return stopRadius; } }

    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;

    int moveHash;

    bool needRandom = true;
    float teleportTime;
    PlayerBattle player;

    // Use this for initialization
    void Start () {
        this.UpdateAsObservable().Subscribe(__ => UniRxUpdate());
		
        anim = GetComponent<Animator>();
        player = GameObject.FindObjectOfType<PlayerBattle>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        moveHash = Animator.StringToHash("EnemyBase.Move");
    }

	// Update is called once per frame
	void UniRxUpdate()
    {
        anim.SetBool("Move", false);

        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player && !player.Missing && CanMove)
        {
            agent.destination = player.transform.position;

            float distance = Vector3.Distance(player.transform.position, transform.position);
            anim.SetBool("Move", distance > stopRadius);

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.fullPathHash == moveHash)
            {
                FaceTarget(agent.steeringTarget);
                agent.nextPosition = transform.position;
            }

            if (distance > teleportMinDistance && distance < teleportMaxDistance)
            {
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
    }

    public void FaceTarget(Vector3 target)
    {
        Vector3 offset = (target == transform.position) ? transform.forward : (target - transform.position).normalized;
        float angle = Vector3.Angle(offset, Vector3.forward) * ((offset.x > 0) ? 1 : -1);
        float diff = Vector3.Angle(offset, transform.forward);
        if (Mathf.Abs(diff) > 5f)
        {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
    
    void Teleport()
    {
        if (player && !player.Missing && CanMove)
        {
            Vector2 offset = Random.insideUnitCircle * stopRadius;
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(player.transform.position + new Vector3(offset.x, 0, offset.y), out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                transform.position = navHit.position;
            }
        }
    }
}
