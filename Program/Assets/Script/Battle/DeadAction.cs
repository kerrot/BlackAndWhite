using UnityEngine;
using System.Collections;

public enum DeadType
{
	DEAD_TYPE_EXPLOSION,
}

public class DeadActionBase{

	public DeadType Type;
	public AttackBase Attack;
	public float Param;
}
