using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Linq;


public class CorePeace : MonoBehaviour
{
    [SerializeField]
    private GameObject Core;
    [SerializeField]
    private KnightBattle battle;

    static private float STOP_DISTANCE = 0.01f;

    static private Subject<Vector3> unionSubject = new Subject<Vector3>();
    static public IObservable<Vector3> OnUnion { get { return unionSubject; } }

    static private Subject<Unit> breakSubject = new Subject<Unit>();
    static public IObservable<Unit> OnBreak { get { return breakSubject; } }

    private Subject<Unit> reachSubject = new Subject<Unit>();

    Animator anim;
    Rigidbody rd;

    public bool Ready { get { return ready; } }
    bool ready = false;

    Vector3 readyPosition;

    System.IDisposable unionDis;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rd = GetComponent<Rigidbody>();

        if (battle)
        {
            battle.OnRevive.Subscribe(_ => Back()).AddTo(this);
        }

        OnBreak.Subscribe(_ => Break()).AddTo(this);
    }

    void OnEnable()
    {
        if (!ready)
        {
            transform.position = Vector3.zero;
            if (anim)
            {
                anim.enabled = true;
                anim.Play("CoreStart", 0);
            }
        }
    }

    void CheckUnion()
    {
        ready = true;
        readyPosition = transform.position;
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = OnUnion.Subscribe(p =>
        {
            Union(p);
            disposable.Dispose();
        }).AddTo(this);

        CorePeace[] ps = GameObject.FindObjectsOfType<CorePeace>();
        if (ps.Length == 3 && ps.All(p => p.Ready))
        {
            Vector3 pos = ps.Select(x => x.transform.position).Aggregate((acc, cur) => acc + cur) / ps.Length;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(pos, out navHit, 1.0f, NavMesh.AllAreas))
            {
                pos = navHit.position;
            }

            unionSubject.OnNext(pos);
        }
    }

    void SetDestinetion(Vector3 pos)
    {
        unionDis = this.UpdateAsObservable().Subscribe(_ =>
        {
            if (rd)
            {
                rd.velocity = pos - transform.position;
                if (Vector3.Distance(transform.position, pos) < STOP_DISTANCE)
                {
                    rd.velocity = Vector3.zero;
                    reachSubject.OnNext(Unit.Default);
                    unionDis.Dispose();
                }
            }
        });
    }

    void Back()
    {
        Core.SetActive(false);
        CorePeace[] ps = GameObject.FindObjectsOfType<CorePeace>();
        if ((ps.Length == 3 && ps.All(p => p.Ready)) || (Core && Core.activeSelf))
        {
            Core.SetActive(false);
            breakSubject.OnNext(Unit.Default);
        }

        if  (unionDis != null)
        {
            unionDis.Dispose();
        }

        ready = false;

        if (anim)
        {
            anim.enabled = false;
        }

        if (transform.parent)
        {
            SetDestinetion(transform.parent.position);

            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = reachSubject.Subscribe(_ =>
            {
                gameObject.SetActive(false);

                if (battle)
                {
                    battle.Revive();
                }

                disposable.Dispose();
            });
        }
    }

    void Break()
    {
        if (!ready)
        {
            return;
        }

        if (unionDis != null)
        {
            unionDis.Dispose();
        }

        gameObject.SetActive(true);

        SetDestinetion(readyPosition);

        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = reachSubject.Subscribe(_ =>
        {
            if (anim)
            {
                anim.enabled = true;
                anim.Play("Core", 0);
            }

            disposable.Dispose();
        });
    }

    void Union(Vector3 pos)
    {
        if (!ready)
        {
            return;
        }

        if (anim)
        {
            anim.enabled = false;
        }

        SetDestinetion(pos);

        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = reachSubject.Subscribe(_ =>
        {
            gameObject.SetActive(false);
            
            if (Core)
            {
                Core.SetActive(true);
                Core.transform.position = pos;
            }

            disposable.Dispose();
        });
    }
}
