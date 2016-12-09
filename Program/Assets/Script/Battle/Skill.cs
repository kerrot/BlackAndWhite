using UnityEngine;
using System.Collections;

public class Skill : MonoBehaviour {
	[SerializeField]
	private float redCost;
	[SerializeField]
	private float greenCost;
	[SerializeField]
	private float blueCost;
	[SerializeField]
	private ElementType type;
	[SerializeField]
	private bool auraType;

	public float RedCost { get { return redCost; } }
	public float GreenCost { get { return greenCost; } }
	public float BlueCost { get { return blueCost; } }
	public ElementType Type { get { return type; } }
	public bool AuraType { get { return auraType; } }

	public virtual void UseSkill()
	{
		
	}

	public virtual bool CanSkill()
	{
		return false;
	}
}
