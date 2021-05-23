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
    [SerializeField]
    private Image attri;

    public Slider HPUI { get { return HP; } }
    public Slider BarrierUI { get { return barrier; } }
    public Slider RecoverUI { get { return recover; } }

    public void SetAttribute(Color color)
    {
        if (attri)
        {
            attri.color = color;
        }
    }
}
