using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Animator anim;

    void Start()
    {
        InputManager.OnMove.Subscribe(dir => Move(dir)).AddTo(this);
    }

    void Move(Vector2 dir)
    {
        if (dir.sqrMagnitude == 0)
        {
            anim.SetBool("Run", false);
        }
        else
        {
            anim.SetBool("Run", true);


            Vector3 lookat = anim.transform.position;
            lookat.x += dir.x;
            lookat.z += dir.y;

            anim.transform.LookAt(lookat);
        }
    }
}
