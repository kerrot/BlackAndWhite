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

        InputController.OnMouseDown.Subscribe(p => StartMove(p));
        InputController.OnMousePressed.Subscribe(p => CheckMotion(p));
        InputController.OnMouseUp.Subscribe(p => StopGuard(p));

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        this.FixedUpdateAsObservable ().Subscribe (_ => UniRxFixedUpdate ());
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

	void UniRxFixedUpdate()
    {
        if (Vector3.Distance(transform.position, TargetObject.transform.position) < arriveRadius)
        {
            anim.SetBool("IsMove", false);
        }
    }

    public Vector3 ComputeDestination(Vector2 position)
    {
        Ray camRay = Camera.main.ScreenPointToRay(position);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            return floorHit.point;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(anim.rootPosition, out navHit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = navHit.position;
        }
    }
}
