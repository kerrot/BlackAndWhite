using UnityEngine;
using System.Collections;

public class TestPhysics : MonoBehaviour {

    [SerializeField]
    private Vector3 force;

	void Start () {
        GetComponent<Rigidbody>().AddForce(force);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
