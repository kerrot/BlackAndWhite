using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField]
    private float time;
    [SerializeField]
    private ParticleSystem effect;

    void Start ()
    {
		if (effect)
        {
            time = effect.main.duration;
        }

        DestroyObject(gameObject, time);
    }
}
