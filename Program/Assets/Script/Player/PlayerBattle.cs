using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBattle : SingletonMonoBehaviour<PlayerBattle> {
    public EnemyGenerator Enermies;
    public GameObject AttackRegion;
    public float AttackAngle = 60;


    float AttackRadius = 1.3f;
    Animator anim;

    AudioSource swing;

    void Start()
    {
		Enermies.OnEnermyClicked += Battle;
        anim = GetComponent<Animator>();
        AttackRadius = AttackRegion.transform.localScale.x / 2;
        swing = GetComponent<AudioSource>();
    }

    void Battle (GameObject enermy)
    {
        if (!PlayerSlash.Instance.SlashEnermy(enermy))
        {
            AttackEnermy(enermy);
        }
    }

	void AttackEnermy(GameObject enermy)
	{
		Vector3 direction = enermy.transform.position - transform.position;
		if (direction.magnitude < AttackRadius) {
			PlayerMove.Instance.CanRotate = false;
			anim.SetTrigger("Attack");
            swing.Play();

        }
	}

    void AttackHit()
    {
        List<GameObject> list = Enermies.GetEnermy(transform.position, AttackRadius, transform.rotation * Vector3.forward, AttackAngle);
        list.ForEach(o =>
        {
            EnermyBattle enermy = o.GetComponent<EnermyBattle>();
            enermy.Attacked(new Attack());
        });

        if (list.Count > 0)
        {
            GameSystem.Instance.Attack();
        }
    }

	void OnDestroy()
	{
		Enermies.OnEnermyClicked -= Battle;
	}
}
