using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class SlashEffect : SingletonMonoBehaviour<SlashEffect>
{
    [SerializeField]
    private TrailRenderer trail;
    [SerializeField]
    private GameObject basePos;
    [SerializeField]
    private GameObject shadow;

    public void EffectStart()
    {
        trail.gameObject.transform.parent = basePos.transform;
        trail.gameObject.transform.localPosition = Vector3.zero;
        trail.enabled = true;
    }

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

    void EffectEnd()
    {
        trail.gameObject.transform.parent = null;

        StartCoroutine(EndTrail());
    }

    IEnumerator EndTrail()
    {
        yield return new WaitForSeconds(trail.time);
        trail.enabled = false;
    }
}
