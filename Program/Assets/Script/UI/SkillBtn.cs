using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBtn : MonoBehaviour {
	[SerializeField]
	private Button powerBtn;
    [SerializeField]
    private GameObject powerOn;
    [SerializeField]
	private Button redBtn;
    [SerializeField]
    private GameObject redOn;
    [SerializeField]
	private Button greenBtn;
    [SerializeField]
    private GameObject greenOn;
    [SerializeField]
	private Button blueBtn;
    [SerializeField]
    private GameObject blueOn;
    [SerializeField]
	private Image center;
    [SerializeField]
    private Image power;

    private Subject<Unit> powerClick = new Subject<Unit>();
	private Subject<bool> redClick = new Subject<bool>();
	private Subject<bool> greenClick = new Subject<bool>();
	private Subject<bool> blueClick = new Subject<bool>();

	public IObservable<Unit> OnPowerClick { get { return powerClick; } }
	public IObservable<bool> OnRedClick { get { return redClick; } }
	public IObservable<bool> OnGreenClick { get { return greenClick; } }
	public IObservable<bool> OnBlueClick { get { return blueClick; } }

	PlayerSkill skill;

	// Use this for initialization
	void Awake () {

		skill = GameObject.FindObjectOfType<PlayerSkill> ();
        if (skill)
        {
            if (powerBtn)
            {
                skill.CanSkill.Subscribe();

            }
        }

		if (powerBtn && powerBtn.gameObject.activeSelf) 
		{
			powerBtn.OnClickAsObservable().Subscribe(u => powerClick.OnNext(Unit.Default));
			InputController.OnSkillClick.Subscribe(u => { if (powerBtn.gameObject.activeSelf) powerClick.OnNext(Unit.Default); }).AddTo(this);
		}

		if (redBtn && redBtn.interactable) 
		{
			redBtn.OnClickAsObservable ().Subscribe (u => redClicked.Value = !redClicked.Value);
			redClicked.Subscribe(v => ButtonClicked(v, redBtn, redClick));
			InputController.OnRedClick.Subscribe(u => redClicked.Value = !redClicked.Value).AddTo(this);
		}
		if (greenBtn && greenBtn.interactable) 
		{
			greenBtn.OnClickAsObservable ().Subscribe (u => greenClicked.Value = !greenClicked.Value);
			greenClicked.Subscribe(v => ButtonClicked(v, greenBtn, greenClick));
			InputController.OnGreenClick.Subscribe(u => greenClicked.Value = !greenClicked.Value).AddTo(this);
		}
		if (blueBtn && blueBtn.interactable) 
		{
			blueBtn.OnClickAsObservable ().Subscribe (u => blueClicked.Value = !blueClicked.Value);
			blueClicked.Subscribe(v => ButtonClicked(v, blueBtn, blueClick));
			InputController.OnBlueClick.Subscribe(u => blueClicked.Value = !blueClicked.Value).AddTo(this);
		}
    }

	void ButtonClicked(bool active, Button btn, Subject<bool> subject)
	{
		subject.OnNext (active);

		if (skill && center) 
		{
			center.color = Attribute.GetColor (skill.CurrentElement, 1f);
			powerBtn.GetComponent<Image> ().color = center.color;
		}
	}
}
