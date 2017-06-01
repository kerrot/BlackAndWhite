using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

//can move by both mouse and key input
public class PlayerMove : UnitMove {
    public static bool CanRotate = true;    //controls whether player can rotate

    [SerializeField]
    private float arriveRadius = 0.1f;
    //[SerializeField]
    //private float GuardRadius = 0.2f;
    [SerializeField]
    private GameObject TargetObject;        // go to the object's position
    [SerializeField]
    private GameObject targetIndex;         // cursor

    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;
    Vector3 keyPosition;                    // // used when move by key input
    int floorMask;

    PlayerSlash slash;

    void Awake() {
		anim = GetComponent<Animator>();

        floorMask = LayerMask.GetMask("Floor");

        InputController.OnMouseDown.Subscribe(p => StartMove(p)).AddTo(this);
        InputController.OnMousePressed.Subscribe(p => CheckMotion(p)).AddTo(this);
        InputController.OnMove.Subscribe(v => KeyMove(v)).AddTo(this);
        InputController.OnStop.Subscribe(v => MoveStop()).AddTo(this);
        //InputController.OnMouseUp.Subscribe(p => StopGuard(p)).AddTo(this);

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        CanRotate = true;

        slash = GetComponent<PlayerSlash>();

        this.FixedUpdateAsObservable ().Subscribe (_ => UniRxFixedUpdate ());
    }

    void StartMove(Vector2 mousePosition)
    {
        if (PlayerBattle.IsDead || slash.IsSlashing)
        {
            return;
        }

        anim.SetBool("IsMove", true);

        SetDestination(mousePosition);
        targetIndex.SetActive(true);
    }

    void KeyMove(Vector2 moveDirection)
    {
        if (PlayerBattle.IsDead || !CanRotate || slash.IsSlashing)
        {
            return;
        }

        keyPosition = transform.position;
        keyPosition.x += moveDirection.x;
        keyPosition.z += moveDirection.y;

        TargetObject.transform.position = keyPosition;

        transform.LookAt(TargetObject.transform);

        anim.SetBool("IsMove", true);
        targetIndex.SetActive(false);
    }

    public void MoveStop()
    {
        anim.SetBool("IsMove", false);
    }

    void CheckMotion(Vector2 mousePosition)
    {
        if (PlayerBattle.IsDead)
        {
            return;
        }
        anim.SetBool("IsMove", true);
        SetDestination(mousePosition);

        //when cannot move, recompute for check guard.
        //bool isGuard = (transform.position - ComputeDestination(mousePosition)).magnitude < GuardRadius;
        //anim.SetBool("Guard", isGuard);
        //anim.SetBool("IsMove", !isGuard);
    }

 //   void StopGuard(Vector2 mousePosition)
	//{
	//	anim.SetBool ("Guard", false);
	//}

	void UniRxFixedUpdate()
    {
        // check to stop
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
        if (!CanRotate || slash.IsSlashing)
        {
            return;
        }

        TargetObject.transform.position = ComputeDestination(position);

        transform.LookAt(TargetObject.transform);
    }

    // for root motion and physics, only change the direction to face
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
				UnityEngine.AI.NavMeshHit navHit;
				if (UnityEngine.AI.NavMesh.SamplePosition(anim.rootPosition, out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    Vector3 pp = navHit.position;
                    pp.y = 0;   
                    transform.position = pp;
				}
			}
		}
    }
}
