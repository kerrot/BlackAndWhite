using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour {

    [SerializeField]
    private string filename;

    private void Start()
    {
        this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0))
                                 .Subscribe(_ => Application.CaptureScreenshot(filename));
    }
}
