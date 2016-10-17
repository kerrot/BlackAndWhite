using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class PlayerMove : SingletonMonoBehaviour<PlayerMove> {
    public bool CanRotate = true;

    public float arriveRadius = 0.1f;
    public float GuardRadius = 0.2f;
    public GameObject TargetObject;

    Animator anim;
    NavMeshAgent agent;

	static int floorMask;
    static float camRayLength = 100f;

	void Awake() {
		anim = GetComponent<Animator>();

        floorMask = LayerMask.GetMask("Floor");

        InputController.OnMouseDown += StartMove;
        InputController.OnMousePressed += CheckMotion;
        InputController.OnMouseUp += StopGuard;

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

	void Start()
	{
		anim.SetBool("IsMove", anim.GetBool("IsMove"));
	}

    public void StartMove(Vector2 mousePosition)
    {
        anim.SetBool("IsMove", true);

        SetDestination(mousePosition);
    }

    public void CheckMotion(Vector2 mousePosition)
    {
        SetDestination(mousePosition);

        //when cannot move, recompute for check guard.
        bool isGuard = (transform.position - ComputeDestination(mousePosition)).magnitude < GuardRadius;
        anim.SetBool("Guard", isGuard);
        anim.SetBool("IsMove", !isGuard);
    }

    public void StopGuard(Vector2 mousePosition)
	{
		anim.SetBool ("Guard", false);
	}

	void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, TargetObject.transform.position) < arriveRadius)
        {
            anim.SetBool("IsMove", false);
        }

        

        Vector3 offset = (agent.steeringTarget == transform.position) ? transform.forward : (agent.steeringTarget - transform.position).normalized;
        float angle = Vector3.Angle(offset, Vector3.forward) * ((offset.x > 0) ? 1 : -1);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        agent.nextPosition = transform.position;
    }

    public static Vector3 ComputeDestination(Vector2 position)
    {
        Ray camRay = Camera.main.ScreenPointToRay(position);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            return floorHit.point;
        }

        return new Vector3();
    }

    void SetDestination(Vector2 position)
    {
        if (!CanRotate)
        {
            return;
        }

        TargetObject.transform.position = ComputeDestination(position);

        agent.destination = TargetObject.transform.position;
    }

	void OnDestroy()
	{
		InputController.OnMouseDown -= StartMove;
		InputController.OnMousePressed -= CheckMotion;
		InputController.OnMouseUp -= StopGuard;
	}
}
