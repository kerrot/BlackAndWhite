using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBattle : SingletonMonoBehaviour<PlayerBattle> {
    public EnemyGenerator Enemies;
    public GameObject AttackRegion;
    public float AttackAngle = 60;


    float AttackRadius = 1.3f;
    Animator anim;

    AudioSource swing;

    void Start()
    {
		Enemies.OnEnemyClicked += Battle;
        anim = GetComponent<Animator>();
        AttackRadius = AttackRegion.transform.localScale.x / 2;
        swing = GetComponent<AudioSource>();
    }

    void Battle (GameObject Enemy)
    {
        if (!PlayerSlash.Instance.SlashEnemy(Enemy))
        {
            AttackEnemy(Enemy);
        }
    }

	void AttackEnemy(GameObject Enemy)
	{
		Vector3 direction = Enemy.transform.position - transform.position;
		if (direction.magnitude < AttackRadius) {
			PlayerMove.Instance.CanRotate = false;
			anim.SetTrigger("Attack");
            swing.Play();
        }
	}

    void AttackHit()
    {
        List<GameObject> list = Enemies.GetEnemy(transform.position, AttackRadius, transform.rotation * Vector3.forward, AttackAngle);
        list.ForEach(o =>
        {
            EnemyBattle Enemy = o.GetComponent<EnemyBattle>();
            Enemy.Attacked(new Attack());
        });

        if (list.Count > 0)
        {
            GameSystem.Instance.Attack();
        }
    }

	void OnDestroy()
	{
		Enemies.OnEnemyClicked -= Battle;
	}
}
