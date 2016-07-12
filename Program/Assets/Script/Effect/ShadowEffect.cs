using UniRx;
using UniRx.Triggers;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShadowEffect : MonoBehaviour {

    [SerializeField]
    private float time;
    [SerializeField]
    private List<MaterialMapping> mapping = new List<MaterialMapping>();

    [Serializable]
    public struct MaterialMapping
    {
        public Material from;
        public Material to;
    }

    private float start;

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        start = Time.realtimeSinceStartup;

        MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer>();


        renders.ToList().ForEach(r =>
        {
				Material[] tmpMaterial = new Material[r.materials.Length];

            for (int i = 0; i < r.materials.Length; ++i)
            {
                MaterialMapping map = mapping.Find(p => r.materials[i].name.Contains(p.from.name));
                if (map.to != null)
                {
						tmpMaterial[i] = map.to;
                }
            }
			
			r.materials = tmpMaterial;
        });
    }

    void UniRxUpdate()
    {
		float diff = Time.realtimeSinceStartup - start;
		float rate = diff / time;

		MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer>();

		renders.ToList().ForEach(r =>
		{
			r.materials.ToList().ForEach(m =>
			{
				Color tmp = m.color;
				tmp.a = 1.0f - rate;
				m.color = tmp;
			});
		});

		if (diff > time)
        {
            Destroy(gameObject);
        }
    }
}
