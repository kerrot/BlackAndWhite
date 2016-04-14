using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public GameObject enermyGenerator;
	public GameObject player;

	EnemyManager enemymgr;
	PlayerMove playermv;

	// Use this for initialization
	void Start () {
		enemymgr = enermyGenerator.GetComponent<EnemyManager> ();
		playermv = player.GetComponent<PlayerMove> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!enemymgr.ProcessInput ()) {
			playermv.ProcessInput ();
		}
	}
}
