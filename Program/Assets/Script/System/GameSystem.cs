using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField] PlayerLock playerLock;
    [SerializeField] PlayerCursor playerCursor;

    void Start()
    {
        playerLock.Cursor = playerCursor.gameObject;
    }
}
