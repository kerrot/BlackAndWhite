using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillUI : SingletonMonoBehaviour<SkillUI>
{
    [SerializeField]
    private GameObject powerBar;
    [SerializeField]
    private Button skillBtn;

    void Start()
    {
        PlayerSkill.OnPowerChnaged += UpdatePowerBar;
    }

	void UpdatePowerBar(bool usingSkill, float power, float maxPower)
    {
		skillBtn.interactable = !usingSkill;
		skillBtn.gameObject.SetActive(usingSkill || power == maxPower);

		float scale = 1.0f + ((float)power / (float)maxPower) * 2.0f;
        powerBar.transform.localScale = new Vector3(scale, scale, scale);
    }

    void OnDestroy()
    {
        PlayerSkill.OnPowerChnaged -= UpdatePowerBar;
    }
}
