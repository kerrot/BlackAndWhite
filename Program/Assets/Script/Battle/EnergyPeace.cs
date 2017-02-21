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
    [SerializeField]
    private AudioClip groundSE;

    static private Subject<EnergyBall> newSubject = new Subject<EnergyBall>();
    static public IObservable<EnergyBall> OnNew { get { return newSubject; } }

    int floorLayer;
    Rigidbody rd;
    float radius;

    System.IDisposable ground;
    System.IDisposable gather;

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

        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;

        FallToGround();

        EnergyPeace.OnNew.Subscribe(e => Regather(e)).AddTo(this);
    }

    void FallToGround()
    {
        if (ground != null)
        {
            ground.Dispose();
        }
        if (gather != null)
        {
            gather.Dispose();
        }

        rd.useGravity = true;
        ground = this.OnTriggerEnterAsObservable().Subscribe(o =>
        {
            if (rd.useGravity)
            {
                if (o.gameObject.layer == floorLayer)
                {
                    AudioHelper.PlaySE(gameObject, groundSE);

                    rd.useGravity = false;
                    rd.velocity = Vector3.zero;

                    transform.position = new Vector3(transform.position.x, radius, transform.position.z);

                    FindGatherTarget();
                    Union();
                }
            }
        });
    }

    void Union()
    {
        if (ground != null)
        {
            ground.Dispose();
        }
        if (gather != null)
        {
            gather.Dispose();
        }

        if (!gatherTarget)
        {
            return;
        }

        FlytoTarget();

        gather = this.OnTriggerStayAsObservable().Subscribe(o => 
        {
            if (gatherTarget && o.gameObject.GetComponent<EnergyBase>() == gatherTarget)
            {
                EnergyPeace p = o.gameObject.GetComponent<EnergyPeace>();
                if (p)
                {
                    p.FormBall();
                }
                else
                {
                    EnergyBall b = o.gameObject.GetComponent<EnergyBall>();
                    if (b)
                    {
                        b.Gather();
                    }
                }

                Destroy(gameObject);
            }
        });
    }

    EnergyBase FindClosest<T>(System.Func<T, bool> predicate) where T : EnergyBase
    {
        T[] set = GameObject.FindObjectsOfType<T>();
        IEnumerable<T> eset = set.Where(predicate);
        if (eset.Count() > 0)
        {
            return eset.Aggregate((curMin, x) => (curMin == null || (Vector3.Distance(x.transform.position, transform.position) < Vector3.Distance(curMin.transform.position, transform.position)) ? x : curMin));

            //T min = null;
            //float distance = Mathf.Infinity;
            //eset.ToObservable().Subscribe(e =>
            //{
            //    if (e)
            //    {
            //        float tmp = Vector3.Distance(e.transform.position, transform.position);
            //        if (tmp < distance)
            //        {
            //            min = e;
            //            distance = tmp;
            //        }
            //    }
            //});

            //return min;
        }

        return null;
    }

    public void FormBall()
    {
        GameObject obj = Instantiate(energyBall.gameObject, transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = GameObject.FindObjectOfType<EnemyManager>().transform;
        EnergyBall ball = obj.GetComponent<EnergyBall>();
        ball.Type = Type;
        newSubject.OnNext(ball);
        Destroy(gameObject);
    }

    void FindGatherTarget()
    {
        // find closest gather target;
        gatherTarget = FindClosest<EnergyBall>(b => b && b.Type == Type && b.gatherCount > 0);
        if (!gatherTarget)
        {
            gatherTarget = FindClosest<EnergyPeace>(p => p && p.Type == Type && p != this);
        }

        if (gatherTarget && gatherTarget.GatherTarget != this)
        {
            while (gatherTarget.GatherTarget && gatherTarget.GatherTarget != this)
            {
                gatherTarget = gatherTarget.GatherTarget;
            }
        }
        else
        {
            gatherTarget = null;
        }
    }

    void FlytoTarget()
    {
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable().Subscribe(_ =>
        {
            if (gatherTarget)
            {
                rd.velocity = (gatherTarget.transform.position - transform.position).normalized * speed;
            }
            else
            {
                disposable.Dispose();
                FallToGround();
            }
        });
    }

    void Regather(EnergyBall ball)
    {
        if (gatherTarget != null && gatherTarget is EnergyBall)
        {
            return;
        }

        if (Type == ball.Type)
        {
            gatherTarget = ball;
            Union();
        }
    }
}
