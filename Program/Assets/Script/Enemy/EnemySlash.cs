using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// control enemy to be slashed
public class EnemySlash : MonoBehaviour {
    [SerializeField]
    private float slashTime;            //can be slashed time
    [SerializeField]
    private Transform lockUICenter;     //Slash UI display position
    [SerializeField]
    private Transform textUICenter;     //Break UI display position
    [SerializeField]
    private GameObject effect;          //effect to clear when break
    [SerializeField]
    private AudioClip breakSE;
    

    private Subject<GameObject> canSlashSubject = new Subject<GameObject>();
    public IObservable<GameObject> OnCanSlash { get { return canSlashSubject; } }

    GameObject lockUI;
    GameObject breakUI;
    bool canSlash = false;
    float slashStartTime;

    Animator anim;
    Animator UIanim;
    Image UIImage;

    const float LOCKUI_NOT_ACTIVE_ALPHA = 0.2f;

    public bool CanSlash { get { return canSlash && GetComponent<Collider>().enabled; } }

    void Start()
    {
        anim = GetComponent<Animator>();

        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            lockUI = ui.CreateLockUI();
            lockUI.SetActive(false);

            breakUI = ui.CreateBreakUI();
            breakUI.SetActive(false);

            UIanim = lockUI.GetComponent<Animator>();
            UIImage = lockUI.GetComponent<Image>();

            gameObject.OnDisableAsObservable().Where(_ => lockUI).Subscribe(_ => CancelSlash());
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
    }

    // update ui position and slash time
    void UniRxUpdate() {
        if (effect)
        {
            effect.SetActive(!CanSlash);
        }
        
        lockUI.SetActive(CanSlash && gameObject.activeSelf);

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
                    // follow the owner corresponding to screen
                    lockUI.transform.position = Camera.main.WorldToScreenPoint(lockUICenter.transform.position);
                    // LockUI alpha change when out of range
                    if (Vector3.Distance(player.transform.position, lockUICenter.position) > player.SlashRadius)
                    {
                        UIanim.SetBool("Play", false);
                        Color tmp = UIImage.color;
                        tmp.a = LOCKUI_NOT_ACTIVE_ALPHA;
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

        // follow the owner corresponding to screen
        if (breakUI.activeSelf)
        {
            breakUI.transform.position = Camera.main.WorldToScreenPoint(textUICenter.transform.position);
        }
    }

    public void CancelSlash()
    {
        if (canSlash)
        {
            canSlash = false;
            PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
            if (player)
            {
                player.RegisterSlashObject(gameObject, false);
            }
            lockUI.SetActive(false);
        }
    }

    public void TriggerSlash()
    {
        canSlash = true;
        slashStartTime = Time.time;
        anim.SetTrigger("Break");
        AudioHelper.PlaySE(gameObject, breakSE);

        PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
        if (player)
        {
            player.RegisterSlashObject(gameObject, true);
        }

        canSlashSubject.OnNext(gameObject);

        breakUI.SetActive(true);
    }

    void UniRxOnDestroy()
    {
        Destroy(lockUI);
        Destroy(breakUI);
        PlayerSlash player = GameObject.FindObjectOfType<PlayerSlash>();
        if (player)
        {
            player.RegisterSlashObject(gameObject, false);
        }
    }
}
