using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyStickButton : MonoBehaviour {

    private Button btn;

	void Start ()
    {
        btn = GetComponent<Button>();
        this.UpdateAsObservable().Where(_ => Input.GetButtonDown("Attack")).Subscribe(_ => btn.onClick.Invoke());
    }
}
