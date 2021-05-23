using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//player skill storm. attack enemy every [period] second
public class CyanSkill : AuraBattle {
	[SerializeField]
	private float strength;
	[SerializeField]
	private float force;
	[SerializeField]
	private float period;

	Dictionary<EnemyBattle, float> enemyMapping = new Dictionary<EnemyBattle, float>();

	protected override void AuraStart()
	{
        DoRecover();
		this.OnTriggerStayAsObservable().Subscribe(o => UniRxOnTriggerStay(o));
	}

	protected override void AuraDisappear()
	{
		Destroy(gameObject);
	}

	void UniRxOnTriggerStay(Collider other)
	{
        // check if the same enemy

		EnemyBattle enemy = other.gameObject.GetComponent<EnemyBattle>();
		if (enemy)
		{
			if (enemyMapping.ContainsKey (enemy)) {
				if (Time.time - enemyMapping [enemy] < period) {
					return;
				}
				enemyMapping [enemy] = Time.time;
			} 
			else 
			{
				enemyMapping.Add (enemy, Time.time);
			}

			enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength, force));
		}
	}
}
