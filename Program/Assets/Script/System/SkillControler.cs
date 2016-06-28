using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class SkillControler : SingletonMonoBehaviour<SkillControler>
{
    [SerializeField]
    private int maxPower;
    [SerializeField]
    private GameObject powerBar;
    [SerializeField]
    private GameObject skillBtn;

    private int power;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {

    }

    public void AddPower(int v)
    {
        power += v;
        if (power > maxPower)
        {
            power = maxPower;
        }

        skillBtn.SetActive(power == maxPower);

        UpdatePowerBar();
    }

    private void UpdatePowerBar()
    {
        float scale = 1.0f + ((float)power / (float)maxPower) * 2.0f;

        powerBar.transform.localScale = new Vector3(scale, scale, scale);
    }
}
