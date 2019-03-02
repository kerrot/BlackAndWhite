using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

// skill ui setup
public class SkillUI : MonoBehaviour {
	[SerializeField]
	private Button powerBtn;
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
    PlayerAttribute attri;

	const float ENERGY_VALUE_RANGE = 0.55f;

	void Awake()
	{
		skill = GameObject.FindObjectOfType<PlayerSkill> ();
		if (skill) 
		{
            // center circle. can use skill
			if (powerImage && powerBtn) 
			{
				skill.CanSkill.Subscribe (v => 
				{
                        powerImage.gameObject.SetActive(v);
						//powerBtn.interactable = v;
				});
			}

            // use skill
			if (powerBtn) 
			{
				powerBtn.OnClickAsObservable ().Subscribe (_ => skill.UseSkill (EnemyManager.GetEnemyByMousePosition(Input.mousePosition)));
				InputController.OnSkillClick.Subscribe (_ => skill.UseSkill (EnemyManager.GetEnemyByMousePosition(Input.mousePosition)));
			}

            // update energy
			RegisterEnergy (redEnergy, skill.RedEnergy);
			RegisterEnergy (greenEnergy, skill.GreenEnergy);
			RegisterEnergy (blueEnergy, skill.BlueEnergy);
		}

        // update Attribute
        attri = GameObject.FindObjectOfType<PlayerAttribute>();
        if (attri)
        {
            RegisterAttribute(redOn, redEnergyOn, attri.RedOn);
            RegisterAttribute(greenOn, greenEnergyOn, attri.GreenOn);
            RegisterAttribute(blueOn, blueEnergyOn, attri.BlueOn);
        }
    }

	void RegisterAttribute(GameObject btn, GameObject energyOn, IObservable<bool> subject)
	{
		if (btn && energyOn) 
		{
			subject.Subscribe (v => 
            {
                btn.SetActive(v);
                energyOn.SetActive(v);

                Color c = Attribute.GetColor(attri.Type, 1.0f);
                if (centerImage)
                {
                    centerImage.gameObject.SetActive(attri.Type != ElementType.ELEMENT_TYPE_NONE);
                    centerImage.color = c;
                }

                if (powerImage)
                {
                    powerImage.color = c;
                }
            });
		}
	}

	void RegisterEnergy(GameObject energy, IObservable<float> subject)
	{
		if (energy) 
		{
			subject.Subscribe (v => 
			{
					float tmp = v / skill.MaxEnergy * ENERGY_VALUE_RANGE + (1f - ENERGY_VALUE_RANGE);
					energy.transform.localScale = new Vector3(tmp, tmp, tmp);
			});
		}
	}
}
