using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour {

    float initVolume;
    float checkTime;
    System.IDisposable subject;

    AudioSource au;
    // Use this for initialization
    void Start ()
    {
        au = GetComponent<AudioSource>();
        initVolume = au.volume;
    }
	
	public void Play()
    {
        if (subject != null)
        {
            subject.Dispose();
        }

        au.volume = initVolume;
        au.Play();
    }

    public void Fadout()
    {
        subject = this.UpdateAsObservable().Where(w => Time.unscaledTime - checkTime > 0.05f)
                      .TakeWhile(t => au.volume > 0).Subscribe(l =>
                      {
                          au.volume -= 0.02f;

                          checkTime = Time.unscaledTime;
                          if (au.volume <= 0)
                          {
                              subject.Dispose();
                          }
                      });
    }

    public void Pause()
    {
        au.Pause();
    }

    public void Stop()
    {
        au.Stop();
    }

    public void Resume()
    {
        au.UnPause();
    }

    public void Change(AudioClip clip)
    {
        au.clip = clip;
    }
}
