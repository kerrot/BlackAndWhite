using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Button))]
public class SkillButtonUI : MonoBehaviour
{
    [SerializeField] GameObject SkillEnable;
    [SerializeField] GameObject SkillOn;
    [SerializeField] SkillEnergyUI Energy;
    Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        Energy.EnergyFull.Subscribe(e =>
        {
            SkillEnable.SetActive(e);
            btn.enabled = e;
            if (e == false) SkillOn.SetActive(e);
        }).AddTo(this);
    }

    void OnClick()
    {
        SkillOn.SetActive(!SkillOn.activeSelf);
        Debug.Log("hit: " + Energy.name);
    }
}
