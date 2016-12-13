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
    private float deadTime;
    [SerializeField]
    private float showHPTime;
    [SerializeField]
    private Transform HPUICenter;

    public FloatReactiveProperty HP = new FloatReactiveProperty(HPMax);
    public FloatReactiveProperty Barrier = new FloatReactiveProperty(barrierStrength);

    private FloatReactiveProperty recover = new FloatReactiveProperty();

    HPBarUI hpUI;
    float showHPStart;

    void Start()
    {
        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            GameObject tmp = ui.CreateHPUI();
            hpUI = tmp.GetComponent<HPBarUI>();
            HP.subscipt(v => hpUI.HPUI.value = v / HPMax);
            Barrier.subscipt(v => hpUI.BarrierUI.value = v / barrierStrength);
            recover.subscipt(v => hpUI.RecoverUI.value = v / recoverTime);
        }

        EnemyBattle battle = GetComponent<EnemyBattle>();
        if (battle)
        {
            battle.OnAttacked.subscript(_ => ());
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
        this.OnDestroyAsObservable().Subscribe(_ => UniRxOnDestroy());
    }

    void UniRxUpdate()
    {
        if (hpUI.gameObject.activeSelf)
        {
            hpUI.transform.position = Camera.main.WorldToScreenPoint(HPUICenter.transform.position);

            if (currentBarrier <= 0)
            {
                hpUI.SetRecoverEnable(true);
                currentRecover += Time.deltaTime;
                if (currentRecover > recoverTime)
                {
                    currentRecover = 0;
                    showHPStart = Time.time;
                    hpUI.SetRecoverEnable(false);
                    currentBarrier = barrierStrength;
                }
            }

            if (currentRecover == 0 && Time.time - showHPStart > showHPTime)
            {
                hpUI.gameObject.SetActive(false);
            }

            hpUI.SetHPCurrent(currentHP);
            hpUI.SetBarrierCurrent(currentBarrier);
            hpUI.SetRecoverCurrent(currentRecover);
        }
    }

    void UniRxOnDestroy()
    {
        if (hpUI)
        {
            Destroy(hpUI.gameObject);
        }
    }
}
