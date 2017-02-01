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

    public bool UnionReach = false;

    static public float STOP_DISTANCE = 0.01f;

    static private Subject<Vector3> unionSubject = new Subject<Vector3>();
    static public IObservable<Vector3> OnUnion { get { return unionSubject; } }

    static private Subject<Unit> breakSubject = new Subject<Unit>();
    static public IObservable<Unit> OnBreak { get { return breakSubject; } }

    private Subject<Unit> reachSubject = new Subject<Unit>();
    private BoolReactiveProperty ready = new BoolReactiveProperty();
    public IObservable<bool> OnReady { get { return ready; } }

    Animator anim;
    Rigidbody rd;

    public bool Ready { get { return ready.Value; } }

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
        if (!ready.Value)
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
        ready.Value = true;
        readyPosition = transform.position;

        CorePeace[] ps = GameObject.FindObjectsOfType<CorePeace>();
        if (ps.Length == 3 && ps.All(p => p.Ready))
        {
            Vector3 pos = ps.Select(x => x.transform.position).Aggregate((acc, cur) => acc + cur) / ps.Length;
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                pos = navHit.position;
            }

            unionSubject.OnNext(pos);
        }
    }

    void SetDestinetion(Vector3 pos)
    {
        if (unionDis != null)
        {
            unionDis.Dispose();
        }

        unionDis = this.UpdateAsObservable().Subscribe(_ =>
        {
            if (rd)
            {
                rd.velocity = (pos - transform.position);
                float tmp = Vector3.Distance(transform.position, pos);
                if (tmp <= STOP_DISTANCE)
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
        CorePeace[] ps = GameObject.FindObjectsOfType<CorePeace>();
        if ((ps.Length == 3 && ps.All(p => p.Ready)) || (Core && Core.activeSelf))
        {
            breakSubject.OnNext(Unit.Default);
        }

        UnionReach = false;
        gameObject.SetActive(true);
        ready.Value = false;

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
        if (!ready.Value)
        {
            return;
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
        if (!ready.Value)
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
                UnionReach = true;
                Core.transform.position = pos;
            }

            disposable.Dispose();
        });
    }

    public void Register()
    {
        OnUnion.Subscribe(p => Union(p)).AddTo(this);
    }
}
