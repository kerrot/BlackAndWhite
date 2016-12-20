using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnergyPeace : EnergyBase
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private EnergyBall energyBall;

    int floorLayer;
    Rigidbody rd;

    void Start()
    {
        floorLayer = LayerMask.NameToLayer("Floor");
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        mat.SetColor("_EmissionColor", Attribute.GetColor(Type, 1.0f));

        Vector2 v = Random.insideUnitCircle;

        rd = GetComponent<Rigidbody>();
        if (rd)
        {
            rd.velocity = Vector3.up * speed + new Vector3(v.x, 0, v.y);
        }

        this.OnTriggerEnterAsObservable().Subscribe(o => UniRxTriggerEnter(o));
    }

    void UniRxTriggerEnter(Collider other)
    {
        if (rd.useGravity)
        {
            if (other.gameObject.layer == floorLayer)
            {
                rd.useGravity = false;
                rd.velocity = Vector3.zero;

                float radis = GetComponent<SphereCollider>().radius;
                if (transform.position.y < radis)
                {
                    transform.position = new Vector3(transform.position.x, radis, transform.position.z);
                }

                FindGatherTarget();
            }
        }
        else if (gatherTarget && other.gameObject.GetComponent<EnergyBase>() == gatherTarget)
        {
            EnergyPeace p = other.gameObject.GetComponent<EnergyPeace>();
            if (p)
            {
                p.FormBall();
            }
            else
            {
                EnergyBall b = other.gameObject.GetComponent<EnergyBall>();
                if (b)
                {
                    b.Gather();
                }
            }

            Destroy(gameObject);
        }
    }

    EnergyBase FindClosest<T>(System.Func<T, bool> predicate) where T : EnergyBase
    {
        T[] set = GameObject.FindObjectsOfType<T>();
        IEnumerable<T> eset = set.Where(predicate);
        if (eset.Count() > 0)
        {
            return eset.Aggregate((curMin, x) => (curMin == null || (Vector3.Distance(x.transform.position, transform.position) < Vector3.Distance(curMin.transform.position, transform.position)) ? x : curMin));
        }

        return null;
    }

    public void FormBall()
    {
        GameObject ball = Instantiate(energyBall.gameObject, transform.position, Quaternion.identity) as GameObject;
        ball.GetComponent<EnergyBall>().Type = Type;
        Destroy(gameObject);
    }

    void FindGatherTarget()
    {
        // find closest gather target;
        gatherTarget = FindClosest<EnergyBall>(b => b.Type == Type && !b.Formed);
        if (!gatherTarget)
        {
            gatherTarget = FindClosest<EnergyPeace>(p => p.Type == Type && p != this);
        }

        if (gatherTarget && gatherTarget.GatherTarget != this)
        {
            while (gatherTarget.GatherTarget && gatherTarget.GatherTarget != this)
            {
                gatherTarget = gatherTarget.GatherTarget;
            }

            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Subscribe(_ =>
            {
                if (gatherTarget)
                {
                    rd.velocity = (gatherTarget.transform.position - transform.position).normalized * speed;
                }
                else
                {
                    rd.velocity = Vector3.zero;
                    rd.useGravity = true;
                    disposable.Dispose();
                }
            });

            // target destroyed before reach
            gatherTarget.OnDestroyAsObservable().Subscribe(_ => FindGatherTarget());
        }
        else
        {
            gatherTarget = null;
        }
    }
}
