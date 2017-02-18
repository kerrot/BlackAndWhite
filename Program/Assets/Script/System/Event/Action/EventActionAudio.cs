using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionAudio : EventAction
{
    [SerializeField]
    private AudioControl au;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioAct act;



    public enum AudioAct
    {
        PLAY,
        PAUSE,
        STOP,
        RESUME,
        CHANGE,
        FADOUT,
    }

    public override void Launch()
    {
        if (au)
        {
            switch (act)
            {
                case AudioAct.PLAY:
                    au.Play();
                    break;
                case AudioAct.PAUSE:
                    au.Pause();
                    break;
                case AudioAct.STOP:
                    au.Stop();
                    break;
                case AudioAct.RESUME:
                    au.Resume();
                    break;
                case AudioAct.CHANGE:
                    au.Change(clip);
                    break;
                case AudioAct.FADOUT:
                    au.Fadout();
                    break;
            }
        }
    }
}
