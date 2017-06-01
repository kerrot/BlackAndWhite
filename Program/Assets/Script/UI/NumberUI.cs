using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// display number 0~9, by sprite
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
        gameObject.SetActive(true);

		if (number >= 0 && number < 10) 
		{
			display.sprite = numbers [number];
		}
	}
}
