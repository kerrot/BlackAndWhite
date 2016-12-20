using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class PlayerMove : UnitMove {
    public static bool CanRotate = true;

    public float arriveRadius = 0.1f;
    public float GuardRadius = 0.2f;
    public GameObject TargetObject;

    Animator anim;
    NavMeshAgent agent;

    int floorMask;

    void Awake() {
		anim = GetComponent<Animator>();

        floorMask = LayerMask.GetMask("Floor");

        InputController.OnMouseDown.Subscribe(p => StartMove(p)).AddTo(this);
        InputController.OnMousePressed.Subscribe(p => CheckMotion(p)).AddTo(this);
        //InputController.OnMouseUp.Subscribe(p => StopGuard(p)).AddTo(this);

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        this.FixedUpdateAsObservable ().Subscribe (_ => UniRxFixedUpdate ());
    }

    void StartMove(Vector2 mousePosition)
    {
        if (PlayerBattle.IsDead)
        {
            return;
        }

        anim.SetBool("IsMove", true);

        SetDestination(mousePosition);
    }

    void CheckMotion(Vector2 mousePosition)
    {
        if (PlayerBattle.IsDead)
        {
            return;
        }

        SetDestination(mousePosition);

        //when cannot move, recompute for check guard.
        //bool isGuard = (transform.position - ComputeDestination(mousePosition)).magnitude < GuardRadius;
        //anim.SetBool("Guard", isGuard);
        //anim.SetBool("IsMove", !isGuard);
    }

    void StopGuard(Vector2 mousePosition)
	{
		anim.SetBool ("Guard", false);
	}

	void UniRxFixedUpdate()
    {
        if (Vector3.Distance(transform.position, TargetObject.transform.position) < arriveRadius)
        {
            anim.SetBool("IsMove", false);
        }
    }

    Vector3 ComputeDestination(Vector2 position)
    {
        Ray camRay = Camera.main.ScreenPointToRay(position);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, Mathf.Infinity, floorMask))
        {
            return floorHit.point;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(position);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                return ray.GetPoint(rayDistance);
            }
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

        transform.LookAt(TargetObject.transform);
    }

    void OnAnimatorMove()
    {
		if (CanMove) 
		{
			if (GetComponent<PlayerSlash>().IsSlashing)
			{
				transform.position = anim.rootPosition;
			}
			else
			{
				NavMeshHit navHit;
				if (NavMesh.SamplePosition(anim.rootPosition, out navHit, 1.0f, NavMesh.AllAreas))
				{
					transform.position = navHit.position;
				}
			}
		}
    }
}
