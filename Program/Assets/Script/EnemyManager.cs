using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;
	public GameObject player;
    public float spawnTime = 3f;

	Animator anim;

	int enermyMask;
	float camRayLength = 100f;

	float inputTime;
	List<GameObject> monsters = new List<GameObject>();

    void Start ()
    {
		enermyMask = LayerMask.GetMask ("Enermy");
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
		anim = player.GetComponent<Animator> ();
    }


    void Spawn ()
    {
		if(monsters.Count > 10 || player == null || enemy == null)
        {
            return;
        }

		Vector3 diection = Random.rotation * Vector3.forward;
		diection.y = 0;

		GameObject obj = Instantiate (enemy, player.transform.position + diection.normalized, Quaternion.Euler (0, 180, 0)) as GameObject;
		if (obj != null) {
			obj.layer = LayerMask.NameToLayer("Enermy");
			monsters.Add(obj);
		}
    }

	void EnermyDie(GameObject enermy)
	{
		if (enermy != null && monsters.Contains (enermy)) {
			monsters.Remove (enermy);
		}
	}

	public bool ProcessInput()
	{
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Create a RaycastHit variable to store information about what was hit by the ray.
		RaycastHit enermyHit;

		if(Physics.Raycast (camRay, out enermyHit, camRayLength, enermyMask)){
			if (enermyHit.collider != null) {
				if (Input.GetMouseButtonDown (0)) {
					inputTime = Time.time;
				} else if (Input.GetMouseButtonUp (0)) {
					if (inputTime != 0 && Time.time - inputTime < 0.3f) {
						anim.SetTrigger ("Attack");
						inputTime = 0;
						return true;
					}
				}
			}
		}

		return false;
	}
}
