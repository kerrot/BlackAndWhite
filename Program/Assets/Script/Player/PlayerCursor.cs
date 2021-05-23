using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UniRx;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] Image CursorImage;
    [SerializeField] float MouseSensitive = 3f;
    [SerializeField] float CursorSpeed = 10f;



    void Start()
    {
        InputManager.OnMousePosition.Subscribe(pos => CursorPosition(pos)).AddTo(this);
        InputManager.OnCursor.Subscribe(dir => CursorMove(dir)).AddTo(this);
    }

    Vector3 lastPos;

    void CursorPosition(Vector3 pos)
    {
        if (Vector3.Distance(lastPos, pos) > MouseSensitive)
        {
            CursorImage.enabled = false;
        }

        if (CursorImage.enabled == false)
        {
            CursorImage.rectTransform.position = pos;
        }

        lastPos = pos;
    }

    void CursorMove(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0)
        {
            CursorImage.enabled = true;
            Vector3 move = new Vector3(dir.x, -dir.y, 0) * CursorSpeed;
            CursorImage.rectTransform.position += move;
        }
    }



    //Vector3 cursorPosition;
    //Vector3 ComputeCursorImagePosition(Vector3 mousePosition)
    //{
    //    cursorPosition.x = mousePosition.x - Screen.width;
    //    cursorPosition.y = mousePosition.y - Screen.height;

    //    Debug.Log("M: " + mousePosition + " c: " + cursorPosition);

    //    return cursorPosition;
    //}
}
