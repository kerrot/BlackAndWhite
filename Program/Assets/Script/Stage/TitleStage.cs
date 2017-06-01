using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// play OP, skip OP
public class TitleStage : MonoBehaviour
{
    [SerializeField]
    private MovieTexture mv;    // op
    [SerializeField]
    private GameObject PV;
    [SerializeField]
    private Image cover;
    [SerializeField]
    private float period;
    [SerializeField]
    private Button btn;
    [SerializeField]
    private GameObject bgm;

    private AudioSource audioSource;
    private Animator anim;

    System.IDisposable subject;
    System.IDisposable aniDis;

    void Start()
    {
        Time.timeScale = 1f;

        anim = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = mv.audioClip;

        OPPlay();

        this.OnDestroyAsObservable().Subscribe(_ =>
        {
            if (aniDis != null)
            {
                aniDis.Dispose();
            }
        });
    }

    // play op, skip with any key input
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
        // start fadout
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

        bgm.SetActive(false);
        bgm.SetActive(true);

        OPPlay();
    }

    void OPPlay()
    {
        // start fadin every [period] second, base on BGM
        aniDis = Observable.Timer(System.TimeSpan.FromSeconds(period)).Subscribe(_ =>
        {
            anim.enabled = true;
            anim.Play("Base Layer.OP", 0, 0);
        });

        // start game
        subject = this.UpdateAsObservable().Where(_ => Input.GetButtonDown("Attack")).Subscribe(_ => btn.onClick.Invoke());
    }
}