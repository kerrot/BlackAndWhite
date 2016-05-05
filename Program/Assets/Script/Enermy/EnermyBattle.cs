using UnityEngine;
using System.Collections;

public class EnermyBattle : MonoBehaviour {
    public UnitAction OnDie;
    public bool CanSlash { get { return Slashable.activeSelf; } }
    public float SlashTime = 3;

    float slashStartTime;

    GameObject Slashable;

    void Start()
    {
        Slashable = transform.FindChild("Slashable").gameObject;
    }

	void FixedUpdate() {
	    if (Time.time - slashStartTime > SlashTime)
        {
            Slashable.SetActive(false);
        }
	}

    public void Attacked(AttackBase attack)
    {
        if (attack.Type == AttackType.ATTACK_TYPE_SLASH && CanSlash)
        {
            OnDie(gameObject);
        }
        else
        {
            Slashable.SetActive(true);
            slashStartTime = Time.time;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && collision.collider.gameObject.tag == "Player")
        {
            
        }
    }
}