using UniRx;
using UnityEngine;
using System.Collections;

public class OpeningRTM : MonoBehaviour {
    [SerializeField]
    private GameObject mainCamera;

	private Subject<Unit> openingEnd = new Subject<Unit>();
	public IObservable<Unit> OnOpeningEnd { get { return openingEnd; } } 

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

		openingEnd.OnNext (Unit.Default);
    }
}
