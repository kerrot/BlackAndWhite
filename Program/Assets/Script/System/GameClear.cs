using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameClear : MonoBehaviour {

	[SerializeField]
	Text scoreUI;

	void Start()
	{
		scoreUI.text = GameSystem.GetScore ().ToString ();
	}
}
