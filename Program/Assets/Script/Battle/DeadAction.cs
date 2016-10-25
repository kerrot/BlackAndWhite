using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeadAction : MonoBehaviour
{
    public Attack A = new Attack();

    void Start()
	{
		Destroy(gameObject, GetComponent<ParticleSystem>().duration);

        List<GameObject> list = PlayerBattle.Instance.Enemies.GetEnemy(transform.position, GetComponent<SphereCollider>().radius);
        foreach (GameObject o in list)
        {
            EnemyBattle battle = o.GetComponent<EnemyBattle>();
            if (battle != null)
            {
                battle.Attacked(A);
            }
        }
    }
}