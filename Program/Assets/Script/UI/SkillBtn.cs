using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBtn : MonoBehaviour {

	private Image powerBtnC;

	public GameObject redBtn;
	public GameObject greenBtn;
	public GameObject blueBtn;
    public Button powerBtn;

    private int colorRed;
	private int colorGreen;
	private int colorBlue;

	private const int COLOR_WHITH = 255;
	private const int COLOR_MAX = 255;
	private const int COLOR_MIN = 0;
	private const int ALPHA = 255;

    public delegate void PowerAction();
    public delegate void ButtonAction(bool pressed);
    public PowerAction OnPowerUsed;
    public ButtonAction OnBlueChanged;
    public ButtonAction OnRedChanged;
    public ButtonAction OnGreenChanged;

    void Awake() {
    }

	// Use this for initialization
	void Start () {
		powerBtnC = GameObject.Find ("PowerBtn").GetComponent<Image> ();     
    }

	void ButtonColorUpDate() {
		if (colorRed > COLOR_MIN || colorGreen > COLOR_MIN || colorBlue > COLOR_MIN) {
			powerBtnC.color = new Color (colorRed, colorGreen, colorBlue, ALPHA);
		} else {
			powerBtnC.color = new Color (COLOR_WHITH, COLOR_WHITH, COLOR_WHITH, ALPHA);
		}

        powerBtn.interactable = blueBtn.activeSelf || redBtn.activeSelf || greenBtn.activeSelf;
    }

    public void OnPowerBtn()
    {
        if (OnPowerUsed != null)
        {
            OnPowerUsed();
        }
    }

	public void OnRedButton( ) { 
		if (colorRed == COLOR_MAX) {
			colorRed = COLOR_MIN;
			redBtn.SetActive (false);
		} else {
			colorRed = COLOR_MAX;
			redBtn.SetActive (true);
		}

        ButtonColorUpDate();

        if (OnRedChanged != null)
        {
            OnRedChanged(redBtn.activeSelf);
        }
    }

	public void OnGreenButton( ) {		
		if (colorGreen == COLOR_MAX) {
			colorGreen = COLOR_MIN;
			greenBtn.SetActive (false);
		} else {
			colorGreen = COLOR_MAX;
			greenBtn.SetActive (true);
		}

        ButtonColorUpDate();

        if (OnGreenChanged != null)
        {
            OnGreenChanged(greenBtn.activeSelf);
        }
    }

	public void OnBlueButton( ) {
		if (colorBlue == COLOR_MAX) {
			colorBlue = COLOR_MIN;
			blueBtn.SetActive (false);
		} else {
			colorBlue = COLOR_MAX;
			blueBtn.SetActive (true);
		}

        ButtonColorUpDate();


        if (OnBlueChanged != null) 
		{
			OnBlueChanged (blueBtn.activeSelf);
		}
	}
}
