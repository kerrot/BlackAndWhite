using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillUI : MonoBehaviour {
	[SerializeField]
	private Button powerBtn;
	[SerializeField]
	private GameObject powerOn;
	[SerializeField]
	private GameObject redOn;
	[SerializeField]
	private GameObject blueOn;
	[SerializeField]
	private GameObject greenOn;
	[SerializeField]
	private Image centerImage;
	[SerializeField]
	private Image powerImage;
	[SerializeField]
	private GameObject redEnergy;
	[SerializeField]
	private GameObject greenEnergy;
	[SerializeField]
	private GameObject blueEnergy;
	[SerializeField]
	private GameObject redEnergyOn;
	[SerializeField]
	private GameObject greenEnergyOn;
	[SerializeField]
	private GameObject blueEnergyOn;

	PlayerSkill skill;

	const float ENERGY_VALUE_RANGE = 0.55f;

	void Awake()
	{
		skill = GameObject.FindObjectOfType<PlayerSkill> ();
		if (skill) 
		{
			if (powerOn && powerBtn) 
			{
				skill.CanSkill.Subscribe (v => 
				{
						powerOn.SetActive(v);
						powerBtn.interactable = v;
				});
			}

			if (powerBtn) 
			{
				powerBtn.OnClickAsObservable ().Subscribe (_ => skill.UseSkill ());
				InputController.OnSkillClick.Subscribe (_ => skill.UseSkill ());
			}

			RegisterAttribute (redOn, skill.RedOn);
			RegisterAttribute (greenOn, skill.GreenOn);
			RegisterAttribute (blueOn, skill.BlueOn);

			RegisterEnergy (redEnergy, redEnergyOn, skill.RedEnergy);
			RegisterEnergy (greenEnergy, greenEnergyOn, skill.GreenEnergy);
			RegisterEnergy (blueEnergy, blueEnergyOn, skill.BlueEnergy);
		}
	}

	void RegisterAttribute(GameObject btn, IObservable<bool> subject)
	{
		if (btn) 
		{
			subject.Subscribe (v => btn.SetActive (v));
		}

		Color c = Attribute.GetColor (skill.CurrentElement, 1.0f);

		if (centerImage) 
		{
			centerImage.color = c;
		}

		if (powerImage) 
		{
			powerImage.color = c;
		}
	}

	void RegisterEnergy(GameObject energy, GameObject energyOn, IObservable<float> subject)
	{
		if (energy && energyOn) 
		{
			subject.Subscribe (v => 
			{
					float tmp = v / skill.maxEnergy * ENERGY_VALUE_RANGE + (1f - ENERGY_VALUE_RANGE);
					energy.transform.localScale = new Vector3(tmp, tmp, tmp);
					energyOn.SetActive(tmp == 1.0f);
			});
		}
	}
}
