using UnityEngine;
using System.Collections;

// skill base class
public class Skill : MonoBehaviour {
    // energy cost
	[SerializeField]
    protected float redCost;
	[SerializeField]
    protected float greenCost;
	[SerializeField]
    protected float blueCost;
	[SerializeField]
    protected ElementType type;
	[SerializeField]
    protected bool auraType;    // if the skill is aura type. if not aura type, effect only do once
    [SerializeField]
    protected GameObject skillObject;   // skill effect
    [SerializeField]
    public bool castMotion = true;  // need cast animation?

    public float RedCost { get { return redCost; } }
	public float GreenCost { get { return greenCost; } }
	public float BlueCost { get { return blueCost; } }
	public ElementType Type { get { return type; } }
	public bool AuraType { get { return auraType; } }

    protected PlayerSkill skill;

    //if aura type, check whether the energy enough, or cancel aura effect
    public virtual bool IsUsing()
    {
        return auraType && skillObject.activeSelf;
    }

    //for skill cannot be canceled
    public virtual bool Activated()
    {
        return IsUsing();
    }

    //enable skillObject. or spawn skillObject
    public virtual bool UseSkill()
	{
        if (auraType)
        {
            skillObject.SetActive(true);
            return false;
        }
        else
        {
            GameObject obj = Instantiate(skillObject, transform.position, FindNearestDirection());
            obj.transform.parent = GameObject.FindObjectOfType<EnemyManager>().transform;
            return true;
        }
    }

    public virtual void SkillEnd()
    {
        if (auraType)
        {
            skillObject.SetActive(false);
        }
    }

	public virtual bool CanSkill()
	{
        return CheckEnergy();
    }

    protected bool CheckEnergy()
    {
        if (!skill)
        {
            skill = GameObject.FindObjectOfType<PlayerSkill>();
        }

        if (skill)
        {
            bool result = true;

            if (redCost > 0)
            {
                result &= skill.RedEnergy.Value > 0;
            }

            if (greenCost > 0)
            {
                result &= skill.GreenEnergy.Value > 0;
            }

            if (blueCost > 0)
            {
                result &= skill.BlueEnergy.Value > 0;
            }

            return result;
        }

        return false;
    }

    // auto find the direction of the nearest enemy
    Quaternion FindNearestDirection()
    {
        float distance = Mathf.Infinity;
        GameObject min = gameObject;
        EnemyManager.Enemies.ForEach(e =>
        {
            if (e)
            {
                Collider c = e.GetComponent<Collider>();
                if (c && c.enabled && !c.isTrigger)
                {
                    float tmpDistance = Vector3.Distance(e.transform.position, transform.position);
                    float tmpAngle = Mathf.Abs(Vector3.Angle(transform.forward, e.transform.position - transform.position));
                    if (tmpAngle < 10 && tmpDistance < distance)
                    {
                        distance = tmpDistance;
                        min = e;
                    }
                }
            }
        });

        if (distance != Mathf.Infinity)
        {
            return Quaternion.LookRotation(min.transform.position - transform.position);
        }

        return transform.rotation;
    }
}
