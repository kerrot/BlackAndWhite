using UnityEngine;
using System.Collections;

// sound effect control
public class AudioHelper : MonoBehaviour {

	public static void PlaySE(GameObject obj, AudioClip clip)
    {
        if (obj)
        {
            AudioSource se = obj.GetComponent<AudioSource>();
            if (se && clip)
            {
                se.clip = clip;
                se.Play();
            }
        }
    }
}
