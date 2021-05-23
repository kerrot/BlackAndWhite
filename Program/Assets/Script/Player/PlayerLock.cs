using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UniRx;

public class PlayerLock : MonoBehaviour
{
    [SerializeField] Image LockImage;
    [SerializeField] float baseDistance = 1f;
    [SerializeField] float checkRadius = 0.5f;

    public GameObject Cursor;

    private void Update()
    {
        if (Cursor == null)
        {
            Debug.LogWarning("NoCursor");
            return;
        }

        CursorPosition(Cursor.transform.position);
    }

    void CursorPosition(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.SphereCast(ray, checkRadius, out hit))
        {
            LockImage.enabled = true;

            LockImage.transform.position = Camera.main.WorldToScreenPoint(hit.collider.transform.position);

            float distance = Vector3.Distance(hit.collider.transform.position, Camera.main.transform.position);
            if (distance > 0)
            {
                float scale = baseDistance / distance;
                LockImage.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                LockImage.enabled = false;
            }
        }
        else
        {
            LockImage.enabled = false;
        }
    }
}
