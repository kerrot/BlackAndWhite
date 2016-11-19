using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class InputController : MonoBehaviour {
    [SerializeField]
    private float ClickPeriod = 0.3f;

    static private Subject<Vector2> mouseDoubleClick = new Subject<Vector2>();
    static private Subject<Vector2> mouseSingleClick = new Subject<Vector2>();
    static private Subject<Vector2> mouseDown = new Subject<Vector2>();
    static private Subject<Vector2> mouseUp = new Subject<Vector2>();
    static private Subject<Vector2> mousePressed = new Subject<Vector2>();

    static public IObservable<Vector2> OnMouseDoubleClick { get { return mouseDoubleClick; } }
    static public IObservable<Vector2> OnMouseSingleClick { get { return mouseSingleClick; } }
    static public IObservable<Vector2> OnMouseDown { get { return mouseDown; } }
    static public IObservable<Vector2> OnMouseUp { get { return mouseUp; } }
    static public IObservable<Vector2> OnMousePressed { get { return mousePressed; } }


    private float pressTime;
    private float firstClickTime;
    private float secondClickTime;
    private bool isMousePressed = false;
    private bool pressOnUI = false;

	void Start()
	{
		this.UpdateAsObservable ().Subscribe (_ => UniRxUpdate ());
    }

    //bool IsUGUIHit(Vector3 _scrPos)
    //{ // Input.mousePosition
    //    PointerEventData pointer = new PointerEventData(EventSystem.current);
    //    pointer.position = _scrPos;
    //    List<RaycastResult> result = new List<RaycastResult>();
    //    EventSystem.current.RaycastAll(pointer, result);
    //    return (result.Count > 0);
    //}

    void UniRxUpdate ()
    {
        if (GameSystem.Instance.State != GameSystem.GameState.GAME_STATE_PLAYING)
        {
            return;
        }

        float now = Time.time;
        Vector2 position = Input.mousePosition;

        if (!pressOnUI && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject != null) 
            {
                pressOnUI = true;
                return;
            }

            pressTime = now;
            mouseDown.OnNext(position);
            isMousePressed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pressOnUI) 
            {
                pressOnUI = false;
                return;
            }

            isMousePressed = false;
            firstClickTime = secondClickTime;
            secondClickTime = now;

            if (firstClickTime > 0 && secondClickTime - firstClickTime < ClickPeriod)
            {
                mouseDoubleClick.OnNext(position);
            }

            if (now - pressTime < ClickPeriod)
            {
                mouseSingleClick.OnNext(position);
            }

            mouseUp.OnNext(position);
        }

        if (pressOnUI) 
        {
            return;
        }

        if (isMousePressed)
        {
            mousePressed.OnNext(position);
        }
	}
}
