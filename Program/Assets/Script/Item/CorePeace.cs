using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Linq;

//boss core control, ready to union state, union state
public class CorePeace : MonoBehaviour
{
    [SerializeField]
    private GameObject Core;
    [SerializeField]
    private KnightBattle battle;        // owner

    public bool UnionReach = false;

    static public float STOP_DISTANCE = 0.01f;
    
    // event when core unioned
    static private Subject<Vector3> unionSubject = new Subject<Vector3>();
    static public IObservable<Vector3> OnUnion { get { return unionSubject; } }

    // event when union breaked
    static private Subject<Unit> breakSubject = new Subject<Unit>();
    static public IObservable<Unit> OnBreak { get { return breakSubject; } }

    // event when reach the destinetion
    private Subject<Unit> reachSubject = new Subject<Unit>();
    // state when reacdy to union
    private BoolReactiveProperty ready = new BoolReactiveProperty();
    public IObservable<bool> OnReady { get { return ready; } }

    Animator anim;
    Rigidbody rd;

    const int CORE_AMOUNT = 3;

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
        // ready to union state
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

    //
    void CheckUnion()
    {
        readyPosition = transform.position;
        ready.Value = true;

        // when all ready, chaneg to union state
        CorePeace[] ps = GameObject.FindObjectsOfType<CorePeace>();
        if (ps.Length == CORE_AMOUNT && ps.All(p => p.Ready))
        {
            // compute center posiotn to union
            Vector3 pos = ps.Select(x => x.transform.position).Aggregate((acc, cur) => acc + cur) / ps.Length;
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                pos = navHit.position;
            }

            unionSubject.OnNext(pos);
        }
    }

    //Set the destinetion to reach, and velocity
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
                rd.velocity = (pos - transform.position).normalized;
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

    // return to owner
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

    // break the union
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

    // union state
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
