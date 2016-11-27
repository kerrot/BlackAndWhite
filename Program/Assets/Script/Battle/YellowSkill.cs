using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using System.Collections;

public class YellowSkill : AuraBattle {
	[SerializeField]
	private GameObject thunder;
    [SerializeField]
    private GameObject debuff;
    [SerializeField]
    private float strength;

    static float MAXLENGTH = 10f;

	LineRenderer line;

    Vector3 step;
    Vector3 now;
    Vector3 target;

    protected override void AuraStart()
    {
		line = GetComponent<LineRenderer> ();
		line.SetPosition (0, transform.position);

        target = transform.position + transform.forward * MAXLENGTH;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, MAXLENGTH);
        var objs = hits.Where(h => h.collider.isTrigger == false);
        if (objs.Count() > 0)
        {
            float minValue = objs.Min(o => o.distance);
            RaycastHit min = objs.First(o => o.distance == minValue);
            thunder.SetActive(true);
            thunder.transform.position = min.collider.gameObject.transform.position;
            target = min.point;

            Collider[] cs = Physics.OverlapSphere(thunder.transform.position, thunder.GetComponent<SphereCollider>().radius);
            cs.ToObservable().Subscribe(c =>
            {
                EnemyBattle enemy = c.GetComponent<EnemyBattle>();
                if (enemy)
                {
                    enemy.Attacked(this, CreateAttack(AttackType.ATTACK_TYPE_SKILL, strength));
                    GameObject yellow = Instantiate(debuff, enemy.transform.position, Quaternion.identity) as GameObject;
                    YellowDebuff YD = yellow.GetComponent<YellowDebuff>();
                    if (YD)
                    {
                        YD.vistom = enemy;
                    }
                }
            });
        }

        line.SetPosition(1, target);
        now = transform.position;
        step = (target - transform.position).normalized * 0.1f;

        Observable.FromCoroutine(_ => Disappear(1f)).Subscribe().AddTo(this);
	}

    IEnumerator Disappear(float time)
    {
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
