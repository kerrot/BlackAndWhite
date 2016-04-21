using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public float speed = 1;
	public bool canMove = true;

	bool IsMoving = false;
	Animator anim;
	Rigidbody rb;

	int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
	float camRayLength = 100f;          // The length of the ray from the camera into the scene.

	void Awake() {
		floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	public bool ProcessInput()
	{
		
		IsMoving = Input.GetKey (KeyCode.Mouse0);
		anim.SetBool ("IsMove", IsMoving);

		return true;
	}

	void Update () {
		
	}

	public void StopMove()
	{
		IsMoving = false;
	}

	void FixedUpdate() {
		
		if (IsMoving) {
			Turning ();
			rb.MovePosition (transform.position + transform.forward * speed * Time.deltaTime);
		}
	}

	void Turning () {
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Create a RaycastHit variable to store information about what was hit by the ray.
		RaycastHit floorHit;

		// Perform the raycast and if it hits something on the floor layer...
		if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)){
			// Create a vector from the player to the point on the floor the raycast from the mouse hit.
			Vector3 playerToMouse = floorHit.point - transform.position;

			// Ensure the vector is entirely along the floor plane.
			playerToMouse.y = 0f;

			// Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);

			// Set the player's rotation to this new rotation.
			rb.MoveRotation (newRotation);
		}
	}
}
