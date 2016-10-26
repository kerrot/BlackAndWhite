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

    GameObject lockUI;
    bool canSlash = false;
    float slashStartTime;
    AudioSource se;

    Animator anim;
    Animator UIanim;
    Image UIImage;

    public bool CanSlash { get { return canSlash; } }

    void Start()
    {
        se = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        LockUIGenerator ui = GameObject.FindObjectOfType<LockUIGenerator>();
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
        if (canSlash)
        {
            PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
            if (player)
            {
                if (Time.time - slashStartTime > slashTime)
                {
                    canSlash = false;
                    player.RegisterSlashObject(gameObject, false);
                }
                else
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

            effect.SetActive(!canSlash);
            lockUI.SetActive(canSlash);
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
