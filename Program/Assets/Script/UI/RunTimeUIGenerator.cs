using UnityEngine;
using System.Collections;

public class RunTimeUIGenerator : MonoBehaviour {
    [SerializeField]
    private GameObject lockUI;
    [SerializeField]
    private GameObject HPUI;
    [SerializeField]
    private GameObject breakUI;
    [SerializeField]
    private GameObject blockUI;
    [SerializeField]
    private GameObject hurtUI;

    public GameObject CreateLockUI()
    {
        return CreateUI(lockUI);
    }

    public GameObject CreateHPUI()
    {
        return CreateUI(HPUI);
    }

    public GameObject CreateBreakUI()
    {
        return CreateUI(breakUI);
    }

    public GameObject CreateBlockUI()
    {
        return CreateUI(blockUI);
    }
    public GameObject CreateHurtUI()
    {
        return CreateUI(hurtUI);
    }

    public GameObject CreateUI(GameObject ui)
    {
        if (ui)
        {
            GameObject tmp = Instantiate(ui) as GameObject;
            tmp.transform.SetParent(transform);

            RectTransform t = ui.GetComponent<RectTransform>();
            RectTransform r = tmp.GetComponent<RectTransform>();
            r.sizeDelta = t.sizeDelta;

            return tmp;
        }

        return null;
    }
}
