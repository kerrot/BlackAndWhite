using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// select UI , click UI by key input
public class UISelection : MonoBehaviour {
    [SerializeField]
    private List<SelectButton> btns = new List<SelectButton>();
    [SerializeField]
    private string conformKey = "Attack";
    [SerializeField]
    private string switchKey = "Horizontal";

    IntReactiveProperty now = new IntReactiveProperty();    // current select

    float lastTime;
    const float PERIOD = 0.3f;
    const float SENSITIVE = 0.5f;

    void Start ()
    {
        btns.ForEach(b =>
        {
            b.OnMouseOver.Subscribe(btn => now.Value = btns.IndexOf(btn)).AddTo(this);
        });

        now.Subscribe(i => 
        {
            btns.ForEach(b => b.Select(false));
            btns[i].Select(true);
        });

        // click button
        this.UpdateAsObservable().Where(_ => Input.GetButtonDown(conformKey)).Subscribe(_ => btns[now.Value].Click());

        // switch button
        this.UpdateAsObservable().Where(_ => Mathf.Abs(Input.GetAxis(switchKey)) > SENSITIVE && Time.unscaledTime - lastTime > PERIOD)
                                 .Select(_ => Input.GetAxis(switchKey))
                                 .Subscribe(v =>
                                 {
                                     now.Value = (now.Value + ((v > 0) ? 1 : -1) + btns.Count) % btns.Count;
                                     lastTime = Time.unscaledTime;
                                 });
    }
}
