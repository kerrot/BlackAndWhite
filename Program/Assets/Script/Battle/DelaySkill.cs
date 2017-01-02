using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class DelaySkill : UnitBattle {
	[SerializeField]
	protected ParticleSystem pre;
	[SerializeField]
	protected ParticleSystem main;
	[SerializeField]
	protected float strength;
	[SerializeField]
	protected float force;

	private Subject<Unit> blowSubject = new Subject<Unit> ();
	public IObservable<Unit> OnBlow { get { return blowSubject; } }

    void Start()
	{
		if (pre) 
		{
            Observable.FromCoroutine(Blow).Subscribe();
		}

		DelayStart ();
	}

	protected virtual void DelayStart()
	{
		
	}

	IEnumerator Blow()
	{
		yield return new WaitForSeconds(pre.main.duration);

		if (main) 
		{
			main.gameObject.SetActive (true);
			DestroyObject (gameObject, main.main.duration);
		}

		blowSubject.OnNext (Unit.Default);
	}
}
