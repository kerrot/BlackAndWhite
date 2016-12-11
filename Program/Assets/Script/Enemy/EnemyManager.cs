﻿using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    static private float RayRadius = 0.2f;

    static private Subject<GameObject> enemyClicked = new Subject<GameObject>();
    static private Subject<GameObject> enemyCanSlash = new Subject<GameObject>();
    static private Subject<GameObject> explosionAttacked = new Subject<GameObject>();
    static private Subject<Unit> enemyEmpty = new Subject<Unit>();

    static public IObservable<GameObject> OnEnemyClicked { get { return enemyClicked; } }
    static public IObservable<GameObject> OnEnemyCanSlash { get { return enemyCanSlash; } }
    static public IObservable<GameObject> OnExplosionAttacked { get { return explosionAttacked; } }
    static public IObservable<Unit> OnEnemyEmpty { get { return enemyEmpty; }}

    static public List<GameObject> Enemies { get { return monsters; } }

    static int EnemyMask;
    static float camRayLength = 100f;

    static List<GameObject> monsters = new List<GameObject>();

    void Start ()
    {
		EnemyMask = LayerMask.GetMask ("Enemy");
 
        InputController.OnMouseSingleClick.Subscribe(p => EnemyClicked(p)).AddTo(this);

        GameObject.FindObjectsOfType<EnemyBattle>().ToList().ForEach(e => AddMonster(e.gameObject));

        if (monsters.Count == 0)
        {
			enemyEmpty.OnNext (Unit.Default);
        }
    }

    static public GameObject GetEnemyByMousePosition(Vector2 mousePosition)
    {
        Ray camRay = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] EnemyHit = Physics.SphereCastAll(camRay, RayRadius, camRayLength, EnemyMask);

        if (EnemyHit.Length > 0)
        {
            Plane groundPlane = new Plane(Vector3.up, EnemyHit.First().transform.position);
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 center = ray.GetPoint(rayDistance);

                float min = Mathf.Infinity;
                GameObject enemy = null;

                EnemyHit.ToObservable().Where(h => h.collider.GetComponent<EnemyBattle>() != null).Subscribe(e =>
                {
                    float tmp = Vector3.Distance(e.collider.transform.position, center);
                    if (tmp < min)
                    {
                        enemy = e.collider.gameObject;
                        min = tmp;
                    }
                });

                return enemy;
            }
        }

        return null;
    }


    void EnemyClicked(Vector2 mousePosition)
    {
        GameObject obj = GetEnemyByMousePosition(mousePosition);
        if (obj != null)
        {
            enemyClicked.OnNext(obj);
        }
    }

    

    public GameObject CreateEnemy(GameObject obj, Vector3 position, Quaternion rotation)
    {
        GameObject tmp = Instantiate(obj, position, rotation) as GameObject;
        AddMonster(tmp);

        return tmp;
    }

    void AddMonster(GameObject obj)
    {
        if (obj != null)
        {
            EnemyBattle battle = obj.GetComponentInChildren<EnemyBattle>();
            battle.OnDie.Subscribe(o => EnemyDie(o)).AddTo(this);
            battle.OnExplosionAttacked.Subscribe(o => explosionAttacked.OnNext(o)).AddTo(this);

            EnemySlash slash = obj.GetComponentInChildren<EnemySlash>();
            slash.OnCanSlash.Subscribe(o => enemyCanSlash.OnNext(o)).AddTo(this);

            battle.gameObject.layer = LayerMask.NameToLayer("Enemy");
            monsters.Add(battle.gameObject);
        }
    }

    void EnemyDie(GameObject Enemy)
	{
		if (Enemy != null && monsters.Contains (Enemy)) {
			monsters.Remove (Enemy);

            EnemyBattle battle = Enemy.GetComponent<EnemyBattle>();
            if (battle != null && battle.DeadAction != null)
            {
                GameObject obj = Instantiate(battle.DeadAction, Enemy.transform.position + battle.DeadEffectOffset, Quaternion.identity) as GameObject;
                obj.layer = 0;

                obj.GetComponent<DeadAction>().Atk = battle.DeadAction.GetComponent<DeadAction>().Atk;
            }

//			if (!PlayerSkill.Instance.isSkill) 
//			{
//				PlayerSkill.Instance.AddPower (1);
//			}
            DestroyObject(Enemy);
		}

        if (monsters.Count == 0)
        {
			enemyEmpty.OnNext (Unit.Default);
        }
	}

    public List<GameObject> GetEnemy(Vector3 position, float radius)
    {
        List<GameObject> tmpList = new List<GameObject>();

        foreach (GameObject obj in monsters)
        {
            Vector3 tmpDirection = obj.transform.position - position;
            float dis = tmpDirection.magnitude;
            if (dis <= radius)
            {
                tmpList.Add(obj);
            }
        }

        return tmpList;
    }

    public List<GameObject> GetEnemy(Vector3 position, float radius, Vector3 direction, float angle)
    {
        List<GameObject> radiusList = GetEnemy(position, radius);
        List<GameObject> tmpList = new List<GameObject>();

        foreach (GameObject obj in radiusList)
        {
            Vector3 tmpDirection = obj.transform.position - position;
            if (Vector3.Angle(direction, tmpDirection) <= angle)
            {
                tmpList.Add(obj);
            }
        }

        return tmpList;
    }
}
