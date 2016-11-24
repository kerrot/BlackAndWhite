using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;


public class FireBall : AuraBattle
{
    [SerializeField]
    private AudioClip expolsion;
    [SerializeField]
    private float strength;
    [SerializeField]
    private ParticleSystem explosion;

    float radius;

    protected override void AuraStart()
    {
        radius = GetComponent<SphereCollider>().radius;
        element = ElementType.ELEMENT_TYPE_RED;

        float time = GetComponent<ParticleSystem>().duration;
        Destroy(gameObject, time);

        this.OnParticleCollisionAsObservable().Subscribe(o => UniRxParticleCollision(o));
	}

    void UniRxParticleCollision(GameObject other)
    {
        AudioHelper.PlaySE(gameObject, expolsion);

        Collider[] cs = Physics.OverlapSphere(other.transform.position, radius);
        cs.ToObservable().Subscribe(c =>
        {
            EnemyBattle enemy = c.gameObject.GetComponent<EnemyBattle>();
            if (enemy)
            {
                enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength));
            }
        });

        Destroy(gameObject, explosion.duration);
    }
}
