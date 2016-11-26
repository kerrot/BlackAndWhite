using UnityEngine;
using System.Collections;

public class YellowSkill : MonoBehaviour {
	[SerializeField]
	private GameObject thunder;

	static float MAXLENGTH = 10f;

	LineRenderer line;

	void Start () 
	{
		line = GetComponent<LineRenderer> ();
		line.SetPosition (0, transform.position);

		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, MAXLENGTH)) {
			thunder.SetActive (true);
			thunder.transform.position = hit.collider.gameObject.transform.position;
			line.SetPosition (1, hit.point);
		} 
		else {
			line.SetPosition (1, transform.position + transform.forward * MAXLENGTH);
		}
	}
}
