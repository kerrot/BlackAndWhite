using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class SlashEffect : SingletonMonoBehaviour<SlashEffect>
{
    [SerializeField]
    private GameObject shadow;

    void ShadowEffect()
    {
        GameObject s = Instantiate(shadow) as GameObject;
        s.transform.position = transform.position;
        s.transform.rotation = transform.rotation;

        GameObject target = transform.GetChild(0).gameObject;

        GameObject tmp = Instantiate(target) as GameObject;
        tmp.transform.parent = s.transform;
        tmp.transform.localPosition = target.transform.localPosition;
        tmp.transform.localRotation = target.transform.localRotation;
    }
}
