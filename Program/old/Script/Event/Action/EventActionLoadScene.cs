using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionLoadScene : EventAction
{
    [SerializeField]
    private string str;

    public override void Launch()
    {
        GameScene.LoadScene(str);
    }
}
