using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// button can be clicked by joystick
public class JoyStickButton : MonoBehaviour {
    [SerializeField]
    private string key = "Attack";

    private Button btn;

	void Start ()
    {
        btn = GetComponent<Button>();
        this.UpdateAsObservable().Where(_ => Input.GetButtonDown(key)).Subscribe(_ => btn.onClick.Invoke());
    }
}
