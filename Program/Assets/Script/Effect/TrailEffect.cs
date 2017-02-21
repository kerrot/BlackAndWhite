using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class TrailEffect : MonoBehaviour {
    [SerializeField]
    private MeleeWeaponTrail attackTail;
    [SerializeField]
    private MeleeWeaponTrail slashTail;
    [SerializeField]
    private Transform effectPosition;

    MeleeWeaponTrail attack;
    MeleeWeaponTrail slash;

    public void AttackTrailStart()
    {
        if (!attack)
        {
            attack = TrailCreate(attackTail);
        }
    }

    public void AttackTrailEnd()
    {
        TrailEnd(attack);
        attack = null;
    }

    public void SlashTrailStart()
    {
        if (!slash)
        {
            slashTail.Material.SetColor("_TintColor", Attribute.GetColor(GetComponent<PlayerAttribute>().Type, 1.0f));

            slash = TrailCreate(slashTail);
        }
    }

    public void SlashTrailEnd()
    {
        TrailEnd(slash);
        slash = null;
    }

    MeleeWeaponTrail TrailCreate( MeleeWeaponTrail prefab)
    {
        GameObject obj = Instantiate(prefab.gameObject);
        obj.transform.parent = effectPosition;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        return obj.GetComponent<MeleeWeaponTrail>();
    }

    void TrailEnd(MeleeWeaponTrail instance)
    {
        if (instance)
        {
            instance.transform.parent = transform.parent;
            instance.Emit = false;
        }
    }
}
