using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputController : MonoBehaviour {

    [SerializeField]
    private EventSystem InputMonitor;
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

	void Start()
	{
		this.UpdateAsObservable ().Subscribe (_ => UniRxUpdate ());
    }

    void UniRxUpdate () {
        float now = Time.time;
        Vector2 position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            pressTime = now;
            if (OnMouseDown != null)
            {
                OnMouseDown(position);
            }
            mousePressed = true;

            Debug.Log(InputMonitor.currentSelectedGameObject);
        }

        if (Input.GetMouseButtonUp(0))
        {
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

        if (mousePressed)
        {
            if (OnMousePressed != null)
            {
                OnMousePressed(position);
            }
        }
	}
}
