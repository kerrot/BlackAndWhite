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

    Animator anim;
    Rigidbody rd;

    public bool Ready { get { return ready; } }
    bool ready = false;

    Vector3 readyPosition;

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

    void Back()
    {        
        CorePeace[] ps = GameObject.FindObjectsOfType<CorePeace>();
        if ((ps.Length == 3 && ps.All(p => p.Ready)) || (Core && Core.activeSelf))
        {
            Core.SetActive(false);
            breakSubject.OnNext(Unit.Default);
        }

        ready = false;

        if (anim)
        {
            anim.enabled = false;
        }

        if (rd)
        {
            rd.velocity = -transform.localPosition;
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Subscribe(_ =>
            {
                if (transform.localPosition.magnitude < STOP_DISTANCE)
                {
                    gameObject.SetActive(false);
                    rd.velocity = Vector3.zero;
                    
                    if (battle)
                    {
                        battle.Revive();
                    }

                    disposable.Dispose();
                }
            });
        }
    }

    void Break()
    {
        if (!ready)
        {
            return;
        }

        gameObject.SetActive(true);

        if (rd)
        {
            rd.velocity = readyPosition - transform.position;
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Subscribe(_ =>
            {
                if (Vector3.Distance(transform.position, readyPosition) < STOP_DISTANCE)
                {
                    rd.velocity = Vector3.zero;

                    if (anim)
                    {
                        anim.enabled = true;
                        anim.Play("Core", 0);
                    }

                    disposable.Dispose();
                }
            });
        }
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

        if (rd)
        {
            rd.velocity = pos - transform.position;
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.UpdateAsObservable().Subscribe(_ =>
            {
                if (Vector3.Distance(transform.position, pos) < STOP_DISTANCE)
                {
                    gameObject.SetActive(false);
                    rd.velocity = Vector3.zero;
                    if (Core)
                    {
                        Core.SetActive(true);
                        Core.transform.position = pos;
                    }

                    disposable.Dispose();
                }
            });
        }
    }
}
