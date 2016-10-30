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

    public delegate void MouseAction(Vector2 mousePosition);
    public static MouseAction OnMouseDoubleClick;
    public static MouseAction OnMouseSingleClick;
    public static MouseAction OnMouseDown;
    public static MouseAction OnMouseUp;
    public static MouseAction OnMousePressed;

    private float pressTime;
    private float firstClickTime;
    private float secondClickTime;
    private bool mousePressed = false;
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
            if (OnMouseDown != null)
            {
                OnMouseDown(position);
            }
            mousePressed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pressOnUI) 
            {
                pressOnUI = false;
                return;
            }

            mousePressed = false;
            firstClickTime = secondClickTime;
            secondClickTime = now;

            if (firstClickTime > 0 && secondClickTime - firstClickTime < ClickPeriod)
            {
                if (OnMouseDoubleClick != null)
                {
                    OnMouseDoubleClick(Input.mousePosition);
                }
            }

            if (now - pressTime < ClickPeriod)
            {
                if (OnMouseSingleClick != null)
                {
                    OnMouseSingleClick(position);
                }
            }

            if (OnMouseUp != null)
            {
                OnMouseUp(position);
            }
        }

        if (pressOnUI) 
        {
            return;
        }

        if (mousePressed)
        {
            if (OnMousePressed != null)
            {
                OnMousePressed(position);
            }
        }
	}
}
