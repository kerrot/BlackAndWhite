using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

// main control of input. only trigger event
public class InputController : MonoBehaviour {
    [SerializeField]
    private float ClickPeriod = 0.3f;

    static private Subject<Vector2> mouseDoubleClick = new Subject<Vector2>();
    static private Subject<Vector2> mouseSingleClick = new Subject<Vector2>();
    static private Subject<Vector2> mouseDown = new Subject<Vector2>();
    static private Subject<Vector2> mouseUp = new Subject<Vector2>();
    static private Subject<Vector2> mousePressed = new Subject<Vector2>();
    static private Subject<Vector2> rightMouseDown = new Subject<Vector2>();

    static public UniRx.IObservable<Vector2> OnMouseDoubleClick { get { return mouseDoubleClick; } }
    static public UniRx.IObservable<Vector2> OnMouseSingleClick { get { return mouseSingleClick; } }
    static public UniRx.IObservable<Vector2> OnMouseDown { get { return mouseDown; } }
    static public UniRx.IObservable<Vector2> OnMouseUp { get { return mouseUp; } }
    static public UniRx.IObservable<Vector2> OnMousePressed { get { return mousePressed; } }
    static public UniRx.IObservable<Vector2> OnRightMouseDown { get { return rightMouseDown; } }

    static private Subject<Unit> attackClick = new Subject<Unit>();
    static private Subject<Unit> slashClick = new Subject<Unit>();
    static private Subject<Unit> skillClick = new Subject<Unit>();
    static private Subject<Unit> pauseClick = new Subject<Unit>();
    static private Subject<Vector2> moveByKey = new Subject<Vector2>();
    static private Subject<Unit> stopByKey = new Subject<Unit>();
    static public UniRx.IObservable<Unit> OnAttackClick { get { return attackClick; } }
    static public UniRx.IObservable<Unit> OnSlashClick { get { return slashClick; } }
    static public UniRx.IObservable<Unit> OnSkillClick { get { return skillClick; } }
    static public UniRx.IObservable<Unit> OnPauseClick { get { return pauseClick; } }
    static public UniRx.IObservable<Vector2> OnMove { get { return moveByKey; } }
    static public UniRx.IObservable<Unit> OnStop { get { return stopByKey; } }

    static private Subject<Unit> redClick = new Subject<Unit>();
    static private Subject<Unit> greenClick = new Subject<Unit>();
    static private Subject<Unit> blueClick = new Subject<Unit>();
    static public UniRx.IObservable<Unit> OnRedClick { get { return redClick; } }
    static public UniRx.IObservable<Unit> OnGreenClick { get { return greenClick; } }
    static public UniRx.IObservable<Unit> OnBlueClick { get { return blueClick; } }

    private float pressTime;
    private float firstClickTime;
    private float secondClickTime;
    private bool isMousePressed = false;
    private bool pressOnUI = false;

    private Vector2 moveDirection;
    bool moveFlag = false;

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
        // input only valid in playing state
        if (GameSystem.Instance.State != GameSystem.GameState.GAME_STATE_PLAYING)
        {
            return;
        }

        if (Input.GetButtonDown("Attack"))
        {
            attackClick.OnNext(Unit.Default);
        }

        if (Input.GetButtonDown("Slash"))
        {
            slashClick.OnNext(Unit.Default);
        }

        if (Input.GetButtonDown("Skill"))
        {
            skillClick.OnNext(Unit.Default);
        }

        if (Input.GetButtonDown("Pause"))
        {
            pauseClick.OnNext(Unit.Default);
        }

        moveDirection.y = Input.GetAxis("Vertical");
        moveDirection.x = Input.GetAxis("Horizontal");
        if (moveDirection.magnitude > 0)
        {
            moveFlag = true;
            moveByKey.OnNext(moveDirection);
        }
        else if (moveFlag)
        {
            moveFlag = false;
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

            //double click
            if (firstClickTime > 0 && secondClickTime - firstClickTime < ClickPeriod)
            {
                mouseDoubleClick.OnNext(position);
            }

            // single click
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
