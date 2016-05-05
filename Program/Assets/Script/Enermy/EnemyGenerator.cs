using UnityEngine;
using System.Collections.Generic;

public delegate void UnitAction(GameObject unit);

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemy;
    public float spawnTime = 3f;
    public UnitAction OnEnermyClicked;

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
        RaycastHit enermyHit;
        if (Physics.Raycast(camRay, out enermyHit, camRayLength, enermyMask))
        {
            if (enermyHit.collider != null)
            {
                if (OnEnermyClicked != null)
                {
                    OnEnermyClicked(enermyHit.collider.gameObject);
                }
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
            DestroyObject(enermy);
		}
	}

    public List<GameObject> GetEnermy(Vector3 position, float radius, Vector3 direction, float angle)
    {
        List<GameObject> tmpList = new List<GameObject>();

        foreach (GameObject obj in monsters)
        {
            Vector3 tmpDirection = obj.transform.position - position;
            float dis = tmpDirection.magnitude;
            if (dis <= radius)
            {
                if (Vector3.Angle(direction, tmpDirection) <= angle)
                {
                    tmpList.Add(obj);
                }
            }
        }

        return tmpList;
    }
}
