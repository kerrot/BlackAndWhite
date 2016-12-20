using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class StopMove : MonoBehaviour {
    [SerializeField]
    private float lastTime;
	
	public UnitMove victom { get; set; }

    void Start()
    {
		Destroy(gameObject, lastTime);
		if (victom) 
		{
            transform.parent = victom.transform;

			// every frame set to prevent clear by other
			this.UpdateAsObservable().Subscribe(_ => victom.CanMove = false);
			this.OnDestroyAsObservable ().Subscribe (_ => victom.CanMove = true);	
		}
    }
}
