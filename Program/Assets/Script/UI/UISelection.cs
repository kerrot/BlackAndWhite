using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UISelection : MonoBehaviour {
    [SerializeField]
    private List<SelectButton> btns = new List<SelectButton>();

    IntReactiveProperty now = new IntReactiveProperty();

    float lastTime;
    const float PERIOD = 0.3f;

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

        this.UpdateAsObservable().Where(_ => Input.GetButtonDown("Attack")).Subscribe(_ => btns[now.Value].Click());

        this.UpdateAsObservable().Where(_ => Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5f && Time.unscaledTime - lastTime > PERIOD)
                                 .Select(_ => Input.GetAxis("Horizontal"))
                                 .Subscribe(v =>
                                 {
                                     now.Value = (now.Value + ((v > 0) ? 1 : -1) + btns.Count) % btns.Count;
                                     lastTime = Time.unscaledTime;
                                 });
    }
}
