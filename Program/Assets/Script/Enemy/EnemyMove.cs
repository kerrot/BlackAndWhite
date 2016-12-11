using UniRx;
using UniRx.Triggers;

using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour {

    [SerializeField]
    private float stopRadius;
    [SerializeField]
    private GameObject wanderEffect;
    [SerializeField]
    private float teleportPeriodMin;
    [SerializeField]
    private float teleportPeriodMax;

    public bool CanMove = true;

    public float StopRadius { get { return stopRadius; } }

    Animator anim;
    NavMeshAgent agent;

    int moveHash;

    float teleportTime;
    PlayerBattle player;

    // Use this for initialization
    void Start () {
        this.UpdateAsObservable().Subscribe(__ => UniRxUpdate());
		
        anim = GetComponent<Animator>();
        player = GameObject.FindObjectOfType<PlayerBattle>();

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        moveHash = Animator.StringToHash("EnemyBase.Move");

        teleportTime = Time.time + Random.Range(teleportPeriodMin, teleportPeriodMax);
    }

	// Update is called once per frame
	void UniRxUpdate()
    {
        anim.SetBool("Move", false);

        PlayerBattle player = GameObject.FindObjectOfType<PlayerBattle>();
        if (player && !player.Missing && CanMove)
        {
            agent.destination = player.transform.position;

            anim.SetBool("Move", Vector3.Distance(player.transform.position, transform.position) > stopRadius);

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.fullPathHash == moveHash)
            {
                FaceTarget(agent.steeringTarget);
                agent.nextPosition = transform.position;
            }

            if (Time.time > teleportTime)
            {
                teleportTime = Time.time + Random.Range(teleportPeriodMin, teleportPeriodMax);
                anim.SetTrigger("TelePort");
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
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(player.transform.position + new Vector3(offset.x, 0, offset.y), out navHit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = navHit.position;
            }
        }
    }
}
