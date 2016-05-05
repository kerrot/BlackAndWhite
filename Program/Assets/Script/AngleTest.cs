using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AngleTest : MonoBehaviour {

    public GameObject enemy;
    public float[] angles = new float[10];

    List<GameObject> monsters = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < 10; ++i)
        {
            Vector3 diection = Random.rotation * Vector3.forward;
            diection.y = 0;

            GameObject obj = Instantiate(enemy, transform.position + diection.normalized, Quaternion.Euler(0, 180, 0)) as GameObject;
            if (obj != null)
            {
                obj.name = "M" + i.ToString();

                monsters.Add(obj);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < 10; ++i)
        {
            GameObject obj = monsters[i];

            Vector3 tmpDirection = obj.transform.position - transform.position;
            Vector3 direction = transform.rotation * Vector3.forward;
            angles[i] = Vector3.Angle(direction, tmpDirection);
        }
    }
}
