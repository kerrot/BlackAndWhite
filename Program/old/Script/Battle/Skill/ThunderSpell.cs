using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using System.Collections;

// effect of player skill, Thunder effect occur
public class ThunderSpell : AuraBattle {
	[SerializeField]
	private GameObject thunder;
    [SerializeField]
    private GameObject debuff;
    [SerializeField]
    private float strength;

    static float MAXLENGTH = 10f;

	LineRenderer line;

    // for ray animation
    Vector3 step;       
    Vector3 now;

    Vector3 target;     // the enemy position

    protected override void AuraStart()
    {
        // draw a ray. and attack the first enemy hitted by ray
		line = GetComponent<LineRenderer> ();
		line.SetPosition (0, transform.position);

        target = transform.position + transform.forward * MAXLENGTH;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, MAXLENGTH);
        var objs = hits.Where(h => h.collider.isTrigger == false || h.collider.gameObject.GetComponent<EnemyBattle>() != null);
        if (objs.Count() > 0)
        {
            float minValue = objs.Min(o => o.distance);
            RaycastHit min = objs.First(o => o.distance == minValue);
            thunder.SetActive(true);
            thunder.transform.position = min.point;
            target = min.point;

            // attack all enemies in the range [SphereCollider> radius]
            Collider[] cs = Physics.OverlapSphere(thunder.transform.position, thunder.GetComponent<SphereCollider>().radius);
            cs.ToObservable().Subscribe(c =>
            {
                EnemyBattle enemy = c.GetComponent<EnemyBattle>();
                if (enemy)
                {
                    // stop enemy all action
                    enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength));
                    GameObject yellow = Instantiate(debuff, enemy.transform.position, Quaternion.identity) as GameObject;
                    YellowDebuff YD = yellow.GetComponent<YellowDebuff>();
                    if (YD)
                    {
                        YD.vistom = enemy;
                    }

                    yellow.transform.parent = enemy.transform;
                }
            });
        }

        line.SetPosition(1, target);
        now = transform.position;
        step = (target - transform.position).normalized * 0.1f;

        Observable.FromCoroutine(_ => Disappear(1f)).Subscribe();

        DoRecover();
	}

    IEnumerator Disappear(float time)
    {
        // ray disapear
        yield return new WaitForSeconds(time);
        while (line.enabled)
        {
            if (Vector3.Distance(target, now) > step.magnitude)
            {
                line.SetPosition(0, now);
                now += step;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                line.enabled = false;
            }
        }
    }

    protected override void AuraDisappear()
    {
        Destroy(gameObject);
    }
}
