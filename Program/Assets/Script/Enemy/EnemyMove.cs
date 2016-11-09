using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour {

    [SerializeField]
    private float stopRadius;

    public float StopRadius { get { return stopRadius; } }

    Animator anim;
    NavMeshAgent agent;

    int moveHash;

    // Use this for initialization
    void Start () {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());

        anim = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        moveHash = Animator.StringToHash("EnemyBase.Move");
    }
	
	// Update is called once per frame
	void UniRxUpdate() {
        PlayerMove player = GameObject.FindObjectOfType<PlayerMove>();
        if (player)
        {
            agent.destination = player.transform.position;

            anim.SetBool("Move", Vector3.Distance(player.transform.position, transform.position) > stopRadius);

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.fullPathHash == moveHash)
            {
                FaceTarget(agent.steeringTarget);
                agent.nextPosition = transform.position;
            }
        }
    }

    public void FaceTarget(Vector3 target)
    {
        Vector3 offset = (target == transform.position) ? transform.forward : (target - transform.position).normalized;
        float angle = Vector3.Angle(offset, Vector3.forward) * ((offset.x > 0) ? 1 : -1);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}
