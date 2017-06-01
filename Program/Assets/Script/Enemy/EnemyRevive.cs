using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// revive the enemy when enemy dead in [reviveTime] second
public class EnemyRevive : MonoBehaviour {
    [SerializeField]
    private float reviveTime;
    [SerializeField]
    private GameObject reviver;

    EnemyManager manager;

    class ReviveData
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
    }

    List<ReviveData> revives = new List<ReviveData>();

    void Awake()
    {
        manager = GameObject.FindObjectOfType<EnemyManager>();
        EnemyManager.OnEnemyEmpty.Subscribe(u => revives.Clear()).AddTo(this);

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void Start()
    {
        foreach (Transform t in transform)
        {
            Register(t.gameObject);
        }
    }

    void UniRxUpdate()
    {
        // check the time to revive
        if (manager)
        {
            List<ReviveData> re = revives.FindAll(r => Time.time - r.time > reviveTime);
            re.ForEach(r => Register(manager.CreateEnemy(reviver, r.position, r.rotation)));
            re.ForEach(r => revives.Remove(r));
        }
    }

    void Register(GameObject obj)
    {
        Vector3 position = obj.transform.position;
        Quaternion rotation = obj.transform.rotation;

        obj.OnDestroyAsObservable().Subscribe(d =>
        {
            revives.Add(new ReviveData() { time = Time.time, rotation = rotation, position = position });
        });
    }
}
