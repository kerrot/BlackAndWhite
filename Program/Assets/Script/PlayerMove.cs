using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public float speed = 1;

	bool IsMouseDown = false;
	Vector3 playerMovement;

	Animator anim;
	Rigidbody playerRigidbody;

	int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
	float camRayLength = 100f;          // The length of the ray from the camera into the scene.

	void Awake ()
	{
		floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();
	}

	void FixedUpdate (){
		if (Input.GetMouseButtonDown (0)) {
			IsMouseDown = true;
		
		}
		if (Input.GetMouseButtonUp (0)) {
			IsMouseDown = false;
		}
	


		anim.SetBool ("IsWalking", IsMouseDown);

		if (IsMouseDown) {
			Turning ();
		}
	}

	void Turning ()
	{
		// Create a ray from the mouse cursor on screen in the direction of the camera.
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Create a RaycastHit variable to store information about what was hit by the ray.
		RaycastHit floorHit;

		// Perform the raycast and if it hits something on the floor layer...
		if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
		{
			// Create a vector from the player to the point on the floor the raycast from the mouse hit.
			playerMovement = floorHit.point - transform.position;

			// Ensure the vector is entirely along the floor plane.
			playerMovement.y = 0f;

			// Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
			Quaternion newRotation = Quaternion.LookRotation (playerMovement);

			// Set the player's rotation to this new rotation.
			//playerRigidbody.MoveRotation (newRotation);

			//playerMovement = playerMovement.normalized * speed * Time.deltaTime;
			//playerRigidbody.MovePosition (transform.position + playerMovement);
		}
	}
}
