using UnityEngine;
using System.Collections;

public class RunTimeUIGenerator : MonoBehaviour {
    [SerializeField]
    private GameObject lockUI;
    [SerializeField]
    private GameObject HPUI;

    public GameObject CreateLockUI()
    {
        return CreateUI(lockUI);
    }

    public GameObject CreateHPUI()
    {
        return CreateUI(HPUI);
    }

    GameObject CreateUI(GameObject ui)
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
