using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaterialColor : MonoBehaviour {
    [SerializeField]
    private Color matColor;

	void Start ()
    {
        this.UpdateAsObservable().Subscribe(_ =>
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            renderers.ToObservable().Subscribe(r =>
            {
                Material mat = r.material;
                mat.SetColor("_Color", matColor);
            });
        });		
	}
}
