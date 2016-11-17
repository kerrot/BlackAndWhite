using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class HPBarUI : MonoBehaviour {
    [SerializeField]
    private Slider HP;
    [SerializeField]
    private Slider barrier;
    [SerializeField]
    private Slider recover;

    public void SetHPMax(float v)
    {
        HP.minValue = 0;
        HP.maxValue = v;
    }

    public void SetHPCurrent(float v)
    {
        SetValue(HP, v);
    }

    public void SetBarrierMax(float v)
    {
        barrier.minValue = 0;
        barrier.maxValue = v;
    }

    public void SetBarrierCurrent(float v)
    {
        SetValue(barrier, v);
    }

    public void SetRecoverMax(float v)
    {
        recover.minValue = 0;
        recover.maxValue = v;
    }

    public void SetRecoverCurrent(float v)
    {
        SetValue(recover, v);
    }

    public void SetRecoverEnable(bool v)
    {
        recover.gameObject.SetActive(v);
    }

    public void SetBarrierEnable(bool v)
    {
        barrier.gameObject.SetActive(v);
    }

    void SetValue(Slider s, float v)
    {
        s.value = v;
        if (v < 0)
        {
            s.value = 0;
        }
        else if (v > s.maxValue)
        {
            s.value = s.maxValue;
        }
    }
}
