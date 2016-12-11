using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnergyPeace : EnergyBase
{
    [SerializeField]
    private float gatherRadius;
    [SerializeField]
    private float speed;
    [SerializeField]
    private EnergyBall energyBall;

    public bool Destroying = false;
    public static Subject<EnergyBase> NewBorn = new Subject<EnergyBase>();

    int floorLayer;
    Rigidbody rd;
    EnergyBase target;
    
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
        
        
        this.OnCollisionEnterAsObservable().Subscribe(o => UniRxCollisionEnter(o));

        NewBorn.Subscribe(e =>
        {
            if (!target || target is EnergyPeace)
            {
                if (e.Type == Type && Vector3.Distance(e.transform.position, transform.position) < gatherRadius)
                {
                    target = e;
                    if (rd)
                    {
                        rd.velocity = target.transform.position - transform.position;
                    }
                }
            }
        }).AddTo(this);

    }

    void UniRxCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == floorLayer)
        {
            GetComponent<Collider>().isTrigger = true;
            rd.useGravity = false;
            rd.velocity = Vector3.zero;
            this.OnTriggerEnterAsObservable().Subscribe(o => UniRxTriggerEnter(o));

            // find closest gather target;
            target = FindClosest<EnergyBall>(b => b.Type == Type && !b.Formed && b != this);
            if (!target)
            {
                target = FindClosest<EnergyPeace>(p => p.Type == Type && p != this);  
            }

            if (target)
            {
                rd.velocity = (target.transform.position - transform.position) * speed;
            }
            else
            {
                NewBorn.OnNext(this);
            }
        }
    }

    void UniRxTriggerEnter(Collider other)
    {
        if (target && !Destroying && other.gameObject.GetComponent<EnergyBase>() == target)
        {
            EnergyPeace p = other.gameObject.GetComponent<EnergyPeace>();
            if (p)
            {
                p.Destroying = true;
                GameObject ball = Instantiate(energyBall.gameObject, transform.position, Quaternion.identity) as GameObject;
                ball.GetComponent<EnergyBall>().Type = Type;
                Destroy(p.gameObject);
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
}
