using UnityEngine;
using System.Collections;

public class EnermyBattle : MonoBehaviour {
    public UnitAction OnDie;
    public bool CanSlash { get {
                                return Slashable.activeSelf;
                                } }
    public float SlashTime = 3;
    public GameObject DeadAction;

    float slashStartTime;

    GameObject Slashable;
    NavMeshAgent nav;
    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Slashable = transform.FindChild("Slashable").gameObject;
        nav = GetComponent<NavMeshAgent>();
    }

	void FixedUpdate() {
	    if (Time.time - slashStartTime > SlashTime)
        {
            Slashable.SetActive(false);
        }

        nav.SetDestination(player.position);
    }

    public void Attacked(Attack attack)
    {
        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && CanSlash)
        {
            OnDie(gameObject);
			//StartCoroutine (Bomb);
        }
        else
        {
            Slashable.SetActive(true);
            slashStartTime = Time.time;
        }
    }
	/*
	 * 
	IEnumerator Bomb(){
		yield return  new WaitForSeconds(0.5f);

	}*/
}