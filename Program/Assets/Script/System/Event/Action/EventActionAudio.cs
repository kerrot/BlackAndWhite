using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionAudio : EventAction
{
    [SerializeField]
    private AudioSource au;
    [SerializeField]
    private AudioClip cilp;
    [SerializeField]
    private AudioAct act;

    float initVolume;
    float checkTime;
    System.IDisposable subject;

    public enum AudioAct
    {
        PLAY,
        PAUSE,
        STOP,
        RESUME,
        CHANGE,
        FADOUT,
    }

    private void Start()
    {
        initVolume = au.volume;
    }

    public override void Launch()
    {
        if (au)
        {
            switch (act)
            {
                case AudioAct.PLAY:
                    {
                        if (subject != null)
                        {
                            subject.Dispose();
                        }

                        au.volume = initVolume;
                        au.Play();
                    }
                    break;
                case AudioAct.PAUSE:
                    au.Pause();
                    break;
                case AudioAct.STOP:
                    au.Stop();
                    break;
                case AudioAct.RESUME:
                    au.UnPause();
                    break;
                case AudioAct.CHANGE:
                    au.clip = cilp;
                    break;
                case AudioAct.FADOUT:
                    subject = this.UpdateAsObservable().Where(w => Time.unscaledTime - checkTime > 0.05f)
                                             .TakeWhile(t => au.volume > 0).Subscribe(l =>
                    {
                        au.volume -= 0.01f;
                        checkTime = Time.unscaledTime;
                        if (au.volume <= 0)
                        {
                            subject.Dispose();
                        }
                    });
                    break;
            }
        }
    }
}
