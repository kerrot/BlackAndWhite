using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using UniRx;

public class InputManager : MonoBehaviour
{
    Vector2 posInput;
    Vector2 cursorInput;

    static private Subject<Vector2> moveSubject = new Subject<Vector2>();
    static public IObservable<Vector2> OnMove { get { return moveSubject; } }

    static private Subject<Vector3> mousePositionSubject = new Subject<Vector3>();
    static public IObservable<Vector3> OnMousePosition { get { return mousePositionSubject; } }

    static private Subject<Vector2> cursorSubject = new Subject<Vector2>();
    static public IObservable<Vector2> OnCursor { get { return cursorSubject; } }

    void Update()
    {
        posInput.x = Input.GetAxis("Horizontal");
        posInput.y = Input.GetAxis("Vertical");

        moveSubject.OnNext(posInput);

        Vector3 nouMouse = Input.mousePosition;
        mousePositionSubject.OnNext(nouMouse);


        cursorInput.x = Input.GetAxis("CursorHorizontal");
        cursorInput.y = Input.GetAxis("CursorVertical");
        cursorSubject.OnNext(cursorInput);
    }
}
