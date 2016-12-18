using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject spawnEnemy;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private int maxCount;

    GameSystem system;
    EnemyManager manager;

    List<GameObject> spawns = new List<GameObject>();
    System.IDisposable cancel;

    void Start()
    {
        system = GameObject.FindObjectOfType<GameSystem>();
        manager = GameObject.FindObjectOfType<EnemyManager>();

        if (system && manager && spawnEnemy)
        {
            cancel = Observable.Interval(System.TimeSpan.FromSeconds(spawnTime)).Subscribe(_ => Spawn()).AddTo(this);
        }
    }

    void OnDisable()
    {
        if (cancel != null)
        {
            cancel.Dispose();
        }
    }

    void Spawn()
    {
        if (system.State == GameSystem.GameState.GAME_STATE_PLAYING && (spawns.Count < maxCount || maxCount < 0))
        {
            GameObject obj = manager.CreateEnemy(spawnEnemy, transform.position, transform.rotation);
            if (obj)
            {
                obj.OnDestroyAsObservable().Subscribe(_ => spawns.Remove(obj));
                spawns.Add(obj);
            }
        }
    }
}
