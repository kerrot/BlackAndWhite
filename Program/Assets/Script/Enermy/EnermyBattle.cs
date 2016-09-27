using UnityEngine;
using System.Collections;

public class EnermyBattle : MonoBehaviour {
    public UnitAction OnDie;
    public bool CanSlash { get {
                                return Slashable.activeSelf;
                                } }
    public float SlashTime = 3;
    public GameObject DeadAction;
    public Vector3 DeadEffectOffset;

    float slashStartTime;

    GameObject Slashable;
    NavMeshAgent nav;
    Transform player;

    //Start change to Awake, because Instantiate not call Start but Awake
    void Awake()
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

    public bool Attacked(Attack attack)
    {
        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && CanSlash)
        {
            OnDie(gameObject);
            return true;
        }
        else
        {
            Slashable.SetActive(true);
            slashStartTime = Time.time;
        }

        return false;
    }
}