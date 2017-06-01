using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

// generate shadow when player slash, and slash effect
public class SlashEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject shadow;
    [SerializeField]
    private Transform effectPosition;
    [SerializeField]
    private SlashFlash slashEffect;

    public void ShadowEffect()
    {
        GameObject s = Instantiate(shadow) as GameObject;
        s.transform.position = transform.position;
        s.transform.rotation = transform.rotation;
        s.transform.parent = transform.parent;

        GameObject target = transform.GetChild(0).gameObject;

        GameObject tmp = Instantiate(target) as GameObject;
        tmp.transform.parent = s.transform;
        tmp.transform.localPosition = target.transform.localPosition;
        tmp.transform.localRotation = target.transform.localRotation;
    }

    // call by Animation
    void SlashFlash()
    {
        if (slashEffect && effectPosition)
        {
            GameObject obj = Instantiate(slashEffect.gameObject, effectPosition.transform.position, transform.rotation);
            obj.transform.parent = transform.parent;
            obj.GetComponent<SlashFlash>().type = GetComponent<PlayerAttribute>().Type;
        }

        GetComponent<TrailEffect>().SlashTrailEnd();
    }
}
