using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public delegate void UnitAction(GameObject unit);
public delegate void GameAction();

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    private float RayRadius;

    public GameObject enemy;
    public float spawnTime = 3f;

    public UnitAction OnEnemyClicked;
    public UnitAction OnEnemyCanSlash;
    public UnitAction OnExplosionAttacked;
    public GameAction OnEnemyEmpty;

    public List<GameObject> Enemies { get { return monsters; } }

    int EnemyMask;
	float camRayLength = 100f;

	List<GameObject> monsters = new List<GameObject>();

    void Start ()
    {
		EnemyMask = LayerMask.GetMask ("Enemy");
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
 
        InputController.OnMouseSingleClick += EnemyClicked;

        GameObject.FindObjectsOfType<EnemyBattle>().ToList().ForEach(e => AddMonster(e.gameObject));

        if (monsters.Count == 0 && OnEnemyEmpty != null)
        {
            OnEnemyEmpty();
        }
    }

    void EnemyClicked(Vector2 mousePosition)
    {
        Ray camRay = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] EnemyHit = Physics.SphereCastAll(camRay, RayRadius, camRayLength, EnemyMask);

        if (EnemyHit.Length > 0)
        {
            float min = 0;
            RaycastHit minHit = EnemyHit[0];

            foreach (var hit in EnemyHit)
            {
                float tmp = Vector3.Distance(hit.collider.transform.position, camRay.origin);

                if (min == 0)
                {
                    min = tmp;
                    minHit = hit;
                }
                else if (min > tmp)
                {
                    minHit = hit;
                }
            }

            if (OnEnemyClicked != null)
            {
                OnEnemyClicked(minHit.collider.gameObject);
            }
        }
    }

    void Spawn ()
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (!system || system.State != GameSystem.GameState.GAME_STATE_PLAYING)
        {
            return;
        }

        if (monsters.Count > 10 || enemy == null)
        {
            return;
        }

		Vector3 diection = Random.rotation * Vector3.forward;
		diection.y = 0;

        GameObject obj = Instantiate (enemy, transform.position + diection.normalized, Quaternion.Euler (0, 180, 0)) as GameObject;
        AddMonster(obj);
    }

    void AddMonster(GameObject obj)
    {
        if (obj != null)
        {
            obj.layer = LayerMask.NameToLayer("Enemy");
            monsters.Add(obj);

            EnemyBattle battle = obj.GetComponent<EnemyBattle>();
            battle.OnDie += EnemyDie;
            battle.OnExplosionAttacked += EnemyExplosionAttacked;

            EnemySlash slash = obj.GetComponent<EnemySlash>();
            slash.OnCanSlash += EnemySlashTriggered;
        }
    }

    void EnemyExplosionAttacked(GameObject unit)
    {
        if (OnExplosionAttacked != null)
        {
            OnExplosionAttacked(unit);
        }
    }

    void EnemySlashTriggered(GameObject unit)
    {
        if (OnEnemyCanSlash != null)
        {
            OnEnemyCanSlash(unit);
        }
    }

    void EnemyDie(GameObject Enemy)
	{
		if (Enemy != null && monsters.Contains (Enemy)) {
			monsters.Remove (Enemy);

            EnemyBattle battle = Enemy.GetComponent<EnemyBattle>();
            if (battle != null)
            {
                GameObject obj = Instantiate(battle.DeadAction, Enemy.transform.position + battle.DeadEffectOffset, Quaternion.identity) as GameObject;
                obj.layer = 0;
            }

			if (!PlayerSkill.Instance.isSkill) 
			{
				PlayerSkill.Instance.AddPower (1);
			}
            DestroyObject(Enemy);
		}

        if (monsters.Count == 0 && OnEnemyEmpty != null)
        {
            OnEnemyEmpty();
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

	void OnDestroy()
	{
		InputController.OnMouseSingleClick -= EnemyClicked;
	}
}
