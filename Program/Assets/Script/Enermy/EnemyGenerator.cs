using UnityEngine;
using System.Collections.Generic;

public delegate void UnitAction(GameObject unit);

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    private float RayRadius;

    public GameObject enemy;
    public float spawnTime = 3f;
    public UnitAction OnEnermyClicked;
    public List<GameObject> Enermies { get { return monsters; } }

    int enermyMask;
	float camRayLength = 100f;

	List<GameObject> monsters = new List<GameObject>();

    void Start ()
    {
		enermyMask = LayerMask.GetMask ("Enermy");
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
 
        InputController.OnMouseSingleClick += EnermyClicked;
    }

    void EnermyClicked(Vector2 mousePosition)
    {
        Ray camRay = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] enermyHit = Physics.SphereCastAll(camRay, RayRadius, camRayLength, enermyMask);

        if (enermyHit.Length > 0)
        {
            float min = 0;
            RaycastHit minHit = enermyHit[0];

            foreach (var hit in enermyHit)
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

            if (OnEnermyClicked != null)
            {
                OnEnermyClicked(minHit.collider.gameObject);
            }
        }
    }

    void Spawn ()
    {
		if(monsters.Count > 10 || enemy == null)
        {
            return;
        }

		Vector3 diection = Random.rotation * Vector3.forward;
		diection.y = 0;

        GameObject obj = Instantiate (enemy, transform.position + diection.normalized, Quaternion.Euler (0, 180, 0)) as GameObject;
		if (obj != null) {
			obj.layer = LayerMask.NameToLayer("Enermy");
            monsters.Add(obj);

            EnermyBattle enermy = obj.GetComponent<EnermyBattle>();
            enermy.OnDie += EnermyDie;
        }
    }

	void EnermyDie(GameObject enermy)
	{
		if (enermy != null && monsters.Contains (enermy)) {
			monsters.Remove (enermy);

            EnermyBattle battle = enermy.GetComponent<EnermyBattle>();
            if (battle != null)
            {
                GameObject obj = Instantiate(battle.DeadAction, enermy.transform.position, Quaternion.identity) as GameObject;
                obj.layer = 0;
            }

			if (!PlayerSkill.Instance.isSkill) 
			{
				PlayerSkill.Instance.AddPower (1);
			}
            DestroyObject(enermy);
		}
	}

    public List<GameObject> GetEnermy(Vector3 position, float radius)
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

    public List<GameObject> GetEnermy(Vector3 position, float radius, Vector3 direction, float angle)
    {
        List<GameObject> radiusList = GetEnermy(position, radius);
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
