using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBall : AuraBattle
{
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private GameObject expolsion;
    [SerializeField]
    private float strength;
    [SerializeField]
    private float speed;

    float radius;
    Rigidbody rd;

    protected override void AuraStart()
    {
        rd = GetComponent<Rigidbody>();
        Vector3 tmp = transform.forward * speed;
        tmp.y = 0;
        rd.velocity = tmp;
        radius = expolsion.GetComponent<SphereCollider>().radius;
        Destroy(gameObject, ball.GetComponent<ParticleSystem>().duration);

        this.OnTriggerEnterAsObservable().Subscribe(o => UniRxTriggerEnter(o));
	}

    void UniRxTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        rd.velocity = Vector3.zero;
        ball.SetActive(false);
        expolsion.SetActive(true);

        Collider[] cs = Physics.OverlapSphere(transform.position, radius);
        cs.ToObservable().Subscribe(c =>
        {
            EnemyBattle enemy = c.gameObject.GetComponent<EnemyBattle>();
            if (enemy)
            {
                enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength));
            }
        });

        Destroy(gameObject, expolsion.GetComponent<ParticleSystem>().duration);
    }
}
