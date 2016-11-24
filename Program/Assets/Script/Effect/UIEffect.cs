using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class UIEffect : MonoBehaviour {
    [SerializeField]
    private GameObject effect;

    GameObject obj;

	// Use this for initialization
	void Start () {
        RunTimeUIGenerator ui = GameObject.FindObjectOfType<RunTimeUIGenerator>();
        if (ui)
        {
            obj = ui.CreateUI(effect);
        }

        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
    }

    void UniRxUpdate()
    {
        obj.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    void OnEnable()
    {
        if (obj)
        {
            obj.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (obj)
        {
            obj.SetActive(false);
        }
    }
}
