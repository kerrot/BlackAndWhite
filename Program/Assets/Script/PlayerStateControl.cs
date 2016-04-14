using UnityEngine;
using System.Collections;

public enum PlayerState
{
	PLAYER_STATE_IDLE,
	PLAYER_STATE_MOVE,
	PLAYER_STATE_ATTACK,
	PLAYER_STATE_SLASHSTART,
	PLAYER_STATE_SLASHING,
	PLAYER_STATE_SLASHEND,
};
	
public class PlayerStateBase
{
	public PlayerState state { get; set; }

	public static bool operator !=(PlayerStateBase a, PlayerState b)
	{
		return a.state != b;
	}

	public static bool operator ==(PlayerStateBase a, PlayerState b)
	{
		return a.state == b;
	}

	public virtual PlayerStateBase ChangeState(PlayerState state, PlayerInput playerLock, Animator anim)
	{
		return this;
	}
}
	
public class PlayerStateIdle : PlayerStateBase
{
	public PlayerStateIdle()
	{
		state = PlayerState.PLAYER_STATE_IDLE;
	}

	public override PlayerStateBase ChangeState(PlayerState target, PlayerInput playerLock, Animator anim)
	{
		switch (target) {
		case PlayerState.PLAYER_STATE_MOVE:
			{
				if (!playerLock.HasLock (PlayerLock.PLAYER_LOCK_MOVE)) {
					anim.SetTrigger ("Move");
					return new PlayerStateMove ();
				}	
			}break;
		case PlayerState.PLAYER_STATE_ATTACK:
			{
				if (!playerLock.HasLock (PlayerLock.PLAYER_LOCK_ACTION)) {
					playerLock.SetLock (PlayerLock.PLAYER_LOCK_MOVE);
					anim.SetTrigger ("Attack");
					return new PlayerStateAttack ();
				}	
			}break;
		}


		return this;
	}
}

public class PlayerStateMove : PlayerStateBase
{
	public PlayerStateMove()
	{
		state = PlayerState.PLAYER_STATE_MOVE;
	}

	public override PlayerStateBase ChangeState(PlayerState target, PlayerInput playerLock, Animator anim)
	{
		switch (target) {
		case PlayerState.PLAYER_STATE_IDLE:
			{
				anim.SetTrigger ("Idle");
				return new PlayerStateIdle ();
			}
		case PlayerState.PLAYER_STATE_ATTACK:
			{
				if (!playerLock.HasLock (PlayerLock.PLAYER_LOCK_ACTION)) {
					playerLock.SetLock (PlayerLock.PLAYER_LOCK_MOVE);
					anim.SetTrigger ("Attack");
					return new PlayerStateAttack ();
				}	
			}break;
		}

		return this;
	}
}

public class PlayerStateAttack : PlayerStateBase
{
	public PlayerStateAttack()
	{
		state = PlayerState.PLAYER_STATE_ATTACK;
	}

	public override PlayerStateBase ChangeState(PlayerState target, PlayerInput playerLock, Animator anim)
	{
		

		return this;
	}
}



public class PlayerStateControl : MonoBehaviour {

	PlayerStateBase state = new PlayerStateIdle();

	PlayerInput playerLock;
	Animator anim;

	void Start () {
		playerLock = GetComponent<PlayerInput> ();
		anim = GetComponent<Animator> ();
	}

	public PlayerStateBase GetCurrentState()
	{
		return state;
	}

	public void SetState(PlayerState target)
	{
		state = state.ChangeState (target, playerLock, anim);
	}
}
