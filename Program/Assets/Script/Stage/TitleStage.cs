using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleStage : MonoBehaviour
{
    [SerializeField]
    private MovieTexture mv;
    [SerializeField]
    private GameObject PV;
    [SerializeField]
    private Image cover;
    [SerializeField]
    private float period;
    [SerializeField]
    private Button btn;

    private AudioSource audioSource;
    private Animator anim;

    System.IDisposable subject;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = mv.audioClip;

        OPPlay();
    }

    void OPStart()
    {
        audioSource.Play();
        mv.Play();

        if (subject != null)
        {
            subject.Dispose();
        }
        subject = this.UpdateAsObservable().Where(_ => Input.anyKey).Subscribe(_ => OPEnd());
    }

    void OPEnd()
    {
        anim.enabled = false;
        if (subject != null)
        {
            subject.Dispose();
        }

        PV.SetActive(false);
        cover.color = Color.clear;
        cover.raycastTarget = false;
        audioSource.Stop();
        mv.Stop();

        OPPlay();
    }

    void OPPlay()
    {
        Observable.Timer(System.TimeSpan.FromSeconds(period)).Subscribe(_ =>
        {
            anim.enabled = true;
            anim.Play("Base Layer.OP", 0, 0);
        });

        subject = this.UpdateAsObservable().Where(_ => Input.GetButtonDown("Attack")).Subscribe(_ => btn.onClick.Invoke());
    }
}