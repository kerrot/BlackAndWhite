using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBtn : MonoBehaviour {
	[SerializeField]
	private Button powerBtn;
	[SerializeField]
	private Button redBtn;
	[SerializeField]
	private Button greenBtn;
	[SerializeField]
	private Button blueBtn;
	[SerializeField]
	private Image center;
	[SerializeField]
	private BoolReactiveProperty powerActive;

	private Subject<Unit> powerClick = new Subject<Unit>();
	private Subject<bool> redClick = new Subject<bool>();
	private Subject<bool> greenClick = new Subject<bool>();
	private Subject<bool> blueClick = new Subject<bool>();

	public IObservable<Unit> OnPowerClick { get { return powerClick; } }
	public IObservable<bool> OnRedClick { get { return redClick; } }
	public IObservable<bool> OnGreenClick { get { return greenClick; } }
	public IObservable<bool> OnBlueClick { get { return blueClick; } }

	private BoolReactiveProperty redClicked = new BoolReactiveProperty ();
	private BoolReactiveProperty greenClicked = new BoolReactiveProperty ();
	private BoolReactiveProperty blueClicked = new BoolReactiveProperty ();

	static float ON_ALPHA = 1f;
	static float OFF_ALPHA = 0.3f;

	PlayerSkill skill;

	// Use this for initialization
	void Start () {

		skill = GameObject.FindObjectOfType<PlayerSkill> ();

		powerActive.Subscribe (v => UpdatePowerBtn ());
		if (powerActive.Value) 
		{
			powerBtn.OnClickAsObservable().Subscribe(u => powerClick.OnNext(Unit.Default));
			InputController.OnSkillClick.Subscribe(u => { if (powerBtn.gameObject.activeSelf) powerClick.OnNext(Unit.Default); }).AddTo(this);
		}

		if (redBtn && redBtn.interactable) 
		{
			redBtn.OnClickAsObservable ().Subscribe (u => redClicked.Value = !redClicked.Value);
			redClicked.Subscribe(v => ButtonClicked(v, redBtn, redClick));
			InputController.OnRedClick.Subscribe(u => ButtonClicked(redClicked.Value, redBtn, redClick)).AddTo(this);
		}
		if (greenBtn && greenBtn.interactable) 
		{
			greenBtn.OnClickAsObservable ().Subscribe (u => greenClicked.Value = !greenClicked.Value);
			greenClicked.Subscribe(v => ButtonClicked(v, greenBtn, greenClick));
			InputController.OnGreenClick.Subscribe(u => ButtonClicked(greenClicked.Value, greenBtn, greenClick)).AddTo(this);
		}
		if (blueBtn && blueBtn.interactable) 
		{
			blueBtn.OnClickAsObservable ().Subscribe (u => blueClicked.Value = !blueClicked.Value);
			blueClicked.Subscribe(v => ButtonClicked(v, blueBtn, blueClick));
			InputController.OnBlueClick.Subscribe(u => ButtonClicked(blueClicked.Value, blueBtn, blueClick)).AddTo(this);
		}
    }

	void UpdatePowerBtn()
	{
		powerBtn.gameObject.SetActive (powerActive.Value && (redClicked.Value || greenClicked.Value || blueClicked.Value));
	}

	void ButtonClicked(bool active, Button btn, Subject<bool> subject)
	{
		Color tmp = btn.image.color;
		tmp.a = (active) ? ON_ALPHA : OFF_ALPHA;
		btn.image.color = tmp; 
		subject.OnNext (active);
		UpdatePowerBtn ();

		if (skill && center) 
		{
			center.color = Attribute.GetColor (skill.CurrentElement, 1f);
			powerBtn.GetComponent<Image> ().color = center.color;
		}
	}
}
