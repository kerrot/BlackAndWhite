using UnityEngine;
using System.Collections;

public enum PlayerLock
{
	PLAYER_LOCK_NONE		= 0,
	PLAYER_LOCK_MOVE 		= 1 ,
	PLAYER_LOCK_ACTION		= 1 << 1,
	PLAYER_LOCK_UI			= 1 << 2,
	PLAYER_LOCK_ALL			= PLAYER_LOCK_MOVE | PLAYER_LOCK_ACTION | PLAYER_LOCK_UI,
};

public class PlayerInput : MonoBehaviour {

	public GameObject enermyGenerator;

	EnemyManager enemymgr;
	PlayerMove playermv;

	int playerLock = (int)PlayerLock.PLAYER_LOCK_NONE;

	// Use this for initialization
	void Start () {
		enemymgr = enermyGenerator.GetComponent<EnemyManager> ();
		playermv = GetComponent<PlayerMove> ();
	}

	// Update is called once per frame
	void Update () {
		if (HasLock(PlayerLock.PLAYER_LOCK_ALL)) {
			return;
		}

		if (!HasLock(PlayerLock.PLAYER_LOCK_ACTION) && !enemymgr.ProcessInput ()) {
			if (!HasLock(PlayerLock.PLAYER_LOCK_MOVE)) {
				playermv.ProcessInput ();
			}
		}
	}

	public void SetLock(PlayerLock flag)
	{
		playerLock |= (int)flag;

		if (HasLock (PlayerLock.PLAYER_LOCK_MOVE)) {
			playermv.StopMove ();
		}
	}

	public void UnLock(PlayerLock flag)
	{
		if (HasLock(flag)) {
			playerLock -= (int)flag;
		}
	}

	public int GetCurrentLock()
	{
		return playerLock;
	}

	public bool HasLock(PlayerLock flag)
	{
		int tmpLock = (int)flag;
		return (playerLock & tmpLock) != 0;
	}
}
