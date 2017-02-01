using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class EnemyHP : MonoBehaviour 
{
    [SerializeField]
    private float HPMax;
    [SerializeField]
    private float barrierStrength;
    [SerializeField]
    private float recoverTime;
    [SerializeField]
    private float showHPTime;
    [SerializeField]
    private Transform HPUICenter;
    [SerializeField]
    private HPBarUI hpUI;
    [SerializeField]
    private bool alwaysShow;
    [SerializeField]
    private bool fixPosition;
    
    public bool CanRecover = true;

    public FloatReactiveProperty HP = new FloatReactiveProperty();
    public FloatReactiveProperty Barrier = new FloatReactiveProperty();

    private FloatReactiveProperty recover = new FloatReactiveProperty();
    private Subject<Unit> recoverSubject = new Subject<Unit>();

    public IObservable<Unit> OnRecover { get { return recoverSubject; } }

    float showHPStart;

    void Start()
    {
        HP.Value = HPMax;
        Barrier.Value = barrierStrength;

        if (!hpUI)
        {
            RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
            if (ui)
            {
                GameObject tmp = ui.CreateHPUI();
                hpUI = tmp.GetComponent<HPBarUI>();
                hpUI.SetAttribute(Attribute.GetColor(Attribute.GetWeak(GetComponent<Attribute>().Type), 1.0f));
            }
        }

        HP.Subscribe(v => hpUI.HPUI.value = v / HPMax);
        Barrier.Subscribe(v => hpUI.BarrierUI.value = v / barrierStrength);
        recover.Subscribe(v => hpUI.RecoverUI.value = v / recoverTime);

        hpUI.gameObject.SetActive(alwaysShow);

        EnemyBattle battle = GetComponent<EnemyBattle>();
        if (battle)
        {
            battle.OnAttacked.Subscribe(_ => 
            {
                showHPStart = Time.time;
                hpUI.gameObject.SetActive(true);
            });
        }

        if (alwaysShow)
        {
            gameObject.OnDisableAsObservable().Where(_ => hpUI).Subscribe(_ => hpUI.gameObject.SetActive(false));
            gameObject.OnEnableAsObservable().Where(_ => hpUI).Subscribe(_ => hpUI.gameObject.SetActive(true));
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
    }

    void UniRxUpdate()
    {
        if (hpUI.gameObject.activeSelf)
        {
            if (!fixPosition)
            {
                hpUI.transform.position = Camera.main.WorldToScreenPoint(HPUICenter.transform.position);
            }

            if (Barrier.Value <= 0)
            {
                hpUI.RecoverUI.gameObject.SetActive(true);

                if (CanRecover)
                {
                    recover.Value += Time.deltaTime;
                }
                if (recover.Value > recoverTime)
                {
                    recover.Value = 0;

                    //avoid immediately disappear when recover complete.
                    showHPStart = Time.time;

                    hpUI.RecoverUI.gameObject.SetActive(alwaysShow);
                    Barrier.Value = barrierStrength;
                    recoverSubject.OnNext(Unit.Default);
                }
            }

            if (recover.Value == 0 && Time.time - showHPStart > showHPTime)
            {
                hpUI.gameObject.SetActive(alwaysShow && gameObject.activeSelf);
            }
        }
    }

    void UniRxOnDestroy()
    {
        if (hpUI)
        {
            Destroy(hpUI.gameObject);
        }
    }

    public void Revive()
    {
        HP.Value = HPMax;
    }
}
