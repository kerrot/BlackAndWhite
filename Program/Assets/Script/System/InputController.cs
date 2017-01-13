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
    static private Subject<Vector2> rightMouseDown = new Subject<Vector2>();

    static public IObservable<Vector2> OnMouseDoubleClick { get { return mouseDoubleClick; } }
    static public IObservable<Vector2> OnMouseSingleClick { get { return mouseSingleClick; } }
    static public IObservable<Vector2> OnMouseDown { get { return mouseDown; } }
    static public IObservable<Vector2> OnMouseUp { get { return mouseUp; } }
    static public IObservable<Vector2> OnMousePressed { get { return mousePressed; } }
    static public IObservable<Vector2> OnRightMouseDown { get { return rightMouseDown; } }

    static private Subject<Unit> attackClick = new Subject<Unit>();
    static private Subject<Unit> slashClick = new Subject<Unit>();
    static private Subject<Unit> skillClick = new Subject<Unit>();
    static private Subject<Vector2> moveByKey = new Subject<Vector2>();
    static private Subject<Unit> stopByKey = new Subject<Unit>();
    static public IObservable<Unit> OnAttackClick { get { return attackClick; } }
    static public IObservable<Unit> OnSlashClick { get { return slashClick; } }
    static public IObservable<Unit> OnSkillClick { get { return skillClick; } }
    static public IObservable<Vector2> OnMove { get { return moveByKey; } }
    static public IObservable<Unit> OnStop { get { return stopByKey; } }

    static private Subject<Unit> redClick = new Subject<Unit>();
    static private Subject<Unit> greenClick = new Subject<Unit>();
    static private Subject<Unit> blueClick = new Subject<Unit>();
    static public IObservable<Unit> OnRedClick { get { return redClick; } }
    static public IObservable<Unit> OnGreenClick { get { return greenClick; } }
    static public IObservable<Unit> OnBlueClick { get { return blueClick; } }

    private float pressTime;
    private float firstClickTime;
    private float secondClickTime;
    private bool isMousePressed = false;
    private bool pressOnUI = false;
    private Vector2 moveDirection;

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

        if (Input.GetKeyDown(KeyCode.A))
        {
            attackClick.OnNext(Unit.Default);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            slashClick.OnNext(Unit.Default);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            skillClick.OnNext(Unit.Default);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            redClick.OnNext(Unit.Default);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            greenClick.OnNext(Unit.Default);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            blueClick.OnNext(Unit.Default);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.y = Input.GetAxis("Vertical");
            moveDirection.x = Input.GetAxis("Horizontal");
            moveByKey.OnNext(moveDirection);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            stopByKey.OnNext(Unit.Default);
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

        if (!pressOnUI && Input.GetMouseButtonDown(1))
        {
            rightMouseDown.OnNext(position);
        }

    }
}
