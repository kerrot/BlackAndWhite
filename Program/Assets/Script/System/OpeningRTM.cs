using UnityEngine;
using System.Collections;

public class OpeningRTM : MonoBehaviour {
    [SerializeField]
    private GameObject mainCamera;

    public delegate void RTMAction();
    public RTMAction OnOpeningEnd;

    void Start()
    {
        GameSystem system = GameObject.FindObjectOfType<GameSystem>();
        if (system)
        {
            system.GamePause();
            system.RTM();
        }
    }


    void OpeningEnd()
    {
        mainCamera.SetActive(true);
        gameObject.SetActive(false);

        if (OnOpeningEnd != null)
        {
            OnOpeningEnd();
        }
    }
}
