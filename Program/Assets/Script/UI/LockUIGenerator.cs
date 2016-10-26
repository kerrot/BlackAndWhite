using UnityEngine;
using System.Collections;

public class LockUIGenerator : MonoBehaviour {
    [SerializeField]
    private GameObject lockUI;

    public GameObject CreateLockUI()
    {
        if (lockUI)
        {
            GameObject ui = Instantiate(lockUI) as GameObject;
            ui.transform.SetParent(transform);
            return ui;
        }

        return null;
    }
}
