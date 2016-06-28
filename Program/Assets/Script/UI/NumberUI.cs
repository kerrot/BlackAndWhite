using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumberUI : MonoBehaviour {
	[SerializeField]
	private Sprite[] numbers;

	Image display;

	// Use this for initialization
	void Awake () {
		display = GetComponent<Image> ();
	}

	public void SetNumber(int number)
	{
		if (number >= 0 && number < 10) 
		{
			display.sprite = numbers [number];
		}
	}
}
