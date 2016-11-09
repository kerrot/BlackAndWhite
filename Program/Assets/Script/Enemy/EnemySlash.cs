using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemySlash : MonoBehaviour {
    [SerializeField]
    private float slashTime;
    [SerializeField]
    private Transform lockUICenter;
    [SerializeField]
    private GameObject effect;

    
    public UnitAction OnCanSlash;

    GameObject lockUI;
    bool canSlash = false;
    float slashStartTime;
    AudioSource se;

    Animator anim;
    Animator UIanim;
    Image UIImage;

    public bool CanSlash { get { return canSlash && GetComponent<Collider>().enabled; } }

    void Start()
    {
        se = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            lockUI = ui.CreateLockUI();
            lockUI.SetActive(false);
            UIanim = lockUI.GetComponent<Animator>();
            UIImage = lockUI.GetComponent<Image>();
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate() {
        effect.SetActive(!CanSlash);
        lockUI.SetActive(CanSlash);

        Immunity im = GetComponent<Immunity>();
        if (im)
        {
            im.enabled = !CanSlash;
        }

        if (canSlash)
        {
            PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
            if (player)
            {
                if (Time.time - slashStartTime > slashTime)
                {
                    canSlash = false;
                }
                
                if (lockUI.activeSelf)
                {
                    lockUI.transform.position = Camera.main.WorldToScreenPoint(lockUICenter.transform.position);
                    if (Vector3.Distance(player.transform.position, lockUICenter.position) > player.SlashRadius)
                    {
                        UIanim.SetBool("Play", false);
                        Color tmp = UIImage.color;
                        tmp.a = 0.2f;
                        UIImage.color = tmp;
                    }
                    else
                    {
                        UIanim.SetBool("Play", true);
                        Color tmp = UIImage.color;
                        tmp.a = 1f;
                        UIImage.color = tmp;
                    }
                }
            }

            if (!CanSlash)
            {
                player.RegisterSlashObject(gameObject, false);
            }
        }
    }

    public void TriggerSlash()
    {
        canSlash = true;
        slashStartTime = Time.time;
        anim.SetTrigger("Break");
        se.Play();

        PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
        if (player)
        {
            player.RegisterSlashObject(gameObject, true);
        }

        if (OnCanSlash != null)
        {
            OnCanSlash(gameObject);
        }
    }

    void OnDestroy()
    {
        Destroy(lockUI);
        PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
        if (player)
        {
            player.RegisterSlashObject(gameObject, false);
        }
    }
}
