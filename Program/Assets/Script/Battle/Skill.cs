using UnityEngine;
using System.Collections;

public class Skill : MonoBehaviour {
	[SerializeField]
    protected float redCost;
	[SerializeField]
    protected float greenCost;
	[SerializeField]
    protected float blueCost;
	[SerializeField]
    protected ElementType type;
	[SerializeField]
    protected bool auraType;
    [SerializeField]
    protected GameObject skillObject;

    public float RedCost { get { return redCost; } }
	public float GreenCost { get { return greenCost; } }
	public float BlueCost { get { return blueCost; } }
	public ElementType Type { get { return type; } }
	public bool AuraType { get { return auraType; } }

    protected PlayerSkill skill;

    public virtual bool IsUsing()
    {
        return auraType && skillObject.activeSelf;
    }

    public virtual bool Activated()
    {
        return IsUsing();
    }

    public virtual bool UseSkill()
	{
        if (auraType)
        {
            skillObject.SetActive(true);
            return false;
        }
        else
        {
            Instantiate(skillObject, transform.position, FindNearestDirection());
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
            return  skill.BlueEnergy.Value >= blueCost && 
                    skill.RedEnergy.Value >= redCost && 
                    skill.GreenEnergy.Value >= greenCost;
        }

        return false;
    }

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
