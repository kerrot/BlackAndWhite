using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		this.OnTriggerEnterAsObservable().Subscribe(o => UniRxOnTriggerEnter(o));
		this.OnTriggerStayAsObservable().Subscribe(o => UniRxOnTriggerStay(o));
	}

	protected override void AuraDisappear()
	{
		Destroy(gameObject);
	}

	void UniRxOnTriggerEnter(Collider other)
	{
		EnemyBattle enemy = other.gameObject.GetComponent<EnemyBattle>();
		if (enemy)
		{
			enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength));
		}
	}

	void UniRxOnTriggerStay(Collider other)
	{
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

            Vector3 direction = transform.position - enemy.transform.position;
            enemy.AddForce(direction.normalized * force);
		}
	}
}
