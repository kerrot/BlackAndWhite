using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRevive : MonoBehaviour {
    [SerializeField]
    private float reviveTime;
    [SerializeField]
    private GameObject reviver;

    EnemyGenerator generator;

    class ReviveData
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
    }

    List<ReviveData> revives = new List<ReviveData>();

    void Awake()
    {
        generator = GameObject.FindObjectOfType<EnemyGenerator>();
        EnemyGenerator.OnEnemyEmpty.Subscribe(u => revives.Clear()).AddTo(this);

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
        if (generator)
        {
            List<ReviveData> re = revives.FindAll(r => Time.time - r.time > reviveTime);
            re.ForEach(r => Register(generator.CreateEnemy(reviver, r.position, r.rotation)));
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
