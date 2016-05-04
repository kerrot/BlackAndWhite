using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public float speed = 1;
    public float arriveRadius = 0.1f;
    public float GuardRadius = 0.2f;
    public GameObject TargetObject;


    Animator anim;
    Rigidbody rb;

	int floorMask;
    int runHash;
    float camRayLength = 100f;

    Vector3 TargetPosition;

	void Awake() {
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

        floorMask = LayerMask.GetMask("Floor");
        runHash = Animator.StringToHash("PlayerBase.Run");

        InputController.OnMouseDown += StartMove;
        InputController.OnMousePressed += CheckMotion;
        InputController.OnMouseUp += StopGuard;
    }

    public void StartMove(Vector2 mousePosition)
    {
        anim.SetBool("IsMove", true);

        SetDestination(mousePosition);
    }

    public void CheckMotion(Vector2 mousePosition)
    {
        SetDestination(mousePosition);

        Vector3 destination = ComputeDestination(mousePosition);
        bool isGuard = (transform.position - destination).magnitude < GuardRadius;
        anim.SetBool("Guard", isGuard);
        anim.SetBool("IsMove", !isGuard);
    }

    public void StopGuard(Vector2 mousePosition)
	{
		anim.SetBool ("Guard", false);
	}
    public void InputStop(Vector2 mousePosition)
    {
        anim.SetBool("Guard", false);
    }

	void FixedUpdate() {
        if ((transform.position - TargetPosition).magnitude < arriveRadius)
        {
            anim.SetBool("IsMove", false);
        }
    }

	void Turning () {
        Vector3 playerToMouse = TargetPosition - transform.position;
        playerToMouse.y = 0f;
        if (playerToMouse.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(playerToMouse);
        }
    }

    Vector3 ComputeDestination(Vector2 position)
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
        TargetPosition = ComputeDestination(position);
        TargetObject.transform.position = TargetPosition;
        Turning();
    }
}
