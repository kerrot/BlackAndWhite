using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the effect of slash animation
public class SlashFlash : MonoBehaviour {
    [SerializeField]
    private float existTime;
    [SerializeField]
    public ElementType type;
    [SerializeField]
    private ParticleSystem par;

	// Use this for initialization
	void Start ()
    {
        DestroyObject(gameObject, existTime);
        
        if (par)
        {
            ParticleSystem.MainModule mod = par.main;
            mod.startColor = Attribute.GetColor(type, 1.0f);
        }	
	}
}
