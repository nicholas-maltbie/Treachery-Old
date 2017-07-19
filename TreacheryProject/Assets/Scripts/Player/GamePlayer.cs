using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Actor))]
[RequireComponent(typeof (PlayerMove))]
[RequireComponent(typeof(Inventory))]
public class GamePlayer : NetworkBehaviour {
	public enum ActionState {FREE, ACTING, COOLDOWN, PAUSED, DEAD};
	public enum PlayerType {EXPLORER, TRAITOR, HERO, MADMAN};
	public struct Action
	{
		public string name;
		public bool canMove;
		public bool canInteract;
		public bool canDrop; 
		public bool canJump;
		public bool canCameraTurn;
		public bool canTurnBody;
		public bool canSwitchItems;
		public bool canUseItems;
		public bool canBeInterrupted;
		public bool canMelee;
		public string actionMessage;
		public GameObject controller;

		public Action(string name, GameObject controller, bool canBeInterrupted) {
			this.name = name;
			this.controller = controller;
			this.canBeInterrupted = canBeInterrupted;
			canMove = false;
			canInteract = false;
			canDrop = true;
			canJump = true;
			canCameraTurn = true;
			canTurnBody = true;
			canSwitchItems = true;
			canUseItems = true;
			canBeInterrupted = true;
			canMelee = false;
			actionMessage = "Acting";
		}
	}

	public Animator animator;
	public PlayerType playerState = PlayerType.EXPLORER;
	public string cooldownMessage;
	public PlayerAttack attacker;
	private Actor playerActor;
	private PlayerMove playerMove;
	private Inventory inventory;
	public bool canSwitch = true, canDrop = true, canUse = true, canMelee = true;
	public float cooldownRemain;
	public float cooldownMax;
	
	private ActionState state = ActionState.FREE;
	private ActionState prevState = ActionState.FREE;
	public Action action;
	private bool prevHeld;

	private Haunt display;

	public ActionState GetActionState() {
		return state;
	}

	void Start() {
		playerActor = GetComponent<Actor> ();
		playerMove = GetComponent<PlayerMove> ();
		inventory = GetComponent<Inventory> ();
	}

	[Command]
	public void CmdUse() {
		inventory.held.GetComponent<Item> ().ServerUse (gameObject);
	}

	[ServerCallback]
	public bool InterruptAction() {
		if (action.canBeInterrupted) {
			action.controller.SendMessage ("Interrupt", action.name);
			return true;
		}
		return false;
	}

	[Command]
	private void CmdDie() {
		InterruptAction ();
		state = ActionState.DEAD;
	}

	[ServerCallback]
	public void SetAction(Action action) {
		this.state = ActionState.ACTING;
		this.action = action;
		RpcSetAction (action);
	}

	[ServerCallback]
	public void EndAction(float cooldown, string message) {
		this.state = ActionState.COOLDOWN;
		this.cooldownRemain = cooldown;
		RpcEndAction(cooldown, message);
	}

	[ClientRpc]
	public void RpcSetAction(Action action) {
		this.state = ActionState.ACTING;
		this.action = action;
	}

	[ClientRpc]
	public void RpcEndAction(float cooldown, string message) {
		this.state = ActionState.COOLDOWN;
		this.cooldownRemain = cooldown;
		this.cooldownMax = cooldown;
		this.cooldownMessage = message;
	}

	public void DisplayHauntInfo(Haunt haunt) {
		display = haunt;
		this.state = ActionState.PAUSED;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	[ClientRpc]
	public void RpcSetPlayerState(PlayerType state) {
		this.playerState = state;
	}

	public void StopHauntPrep() {
		display = null;
		this.state = ActionState.FREE;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	[Command]
	public void CmdHauntReady() {
		HauntManager.PlayerReady (this);
	}

	void OnGUI() {
		if (isLocalPlayer) {
			if (display != null) {
				if (HauntManager.gameState == HauntManager.HauntState.PREP) {
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					GUI.Label (new Rect (Screen.width / 2 - Screen.width / 4, 
						Screen.height / 2 - Screen.height / 4, 
						Screen.width / 2, Screen.height / 4), 
						display.hauntName);
					if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 8, 
						    Screen.height / 2, Screen.width / 4, Screen.height / 8), 
						    "Ready"))
						CmdHauntReady ();
				} else {
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					GUI.Label (new Rect (Screen.width / 2 - Screen.width / 4, 
						Screen.height / 2 - Screen.height / 4, 
						Screen.width / 2, Screen.height / 4), 
						display.GetEndText());
				}
			}

			if (state == ActionState.COOLDOWN) {
				float percent = cooldownRemain / cooldownMax;
				if (percent <= .10f)
					percent = .10f;
				float heightMod = 2.9f;
				Vector2 textSize = GUI.skin.label.CalcSize (new GUIContent (cooldownMessage));
				float width = textSize.x;
				float height = textSize.y;
				Color oldColor = Color.white;
				GUI.color = Color.green;
				GUI.Box (new Rect (Screen.width / 2 - width * 0.55f, Screen.height - height * 2f,
					width * 1.1f * percent, height * 1f), "");
				GUI.color = Color.white;
				GUI.contentColor = Color.white;
				GUI.Label (new Rect (Screen.width / 2 - width / 2, Screen.height - height * 2,
					width, height), cooldownMessage);
			} else if (state == ActionState.ACTING) {
				float percent = 1;
				if (percent <= .10f)
					percent = .10f;
				float heightMod = 2.9f;
				Vector2 textSize = GUI.skin.label.CalcSize (new GUIContent (action.actionMessage));
				float width = textSize.x;
				float height = textSize.y;
				Color oldColor = Color.white;
				GUI.color = Color.green;
				GUI.Box (new Rect (Screen.width / 2 - width * 0.55f, Screen.height - height * 2f,
					width * 1.1f * percent, height * 1f), "");
				GUI.color = Color.white;
				GUI.contentColor = Color.white;
				GUI.Label (new Rect (Screen.width / 2 - width / 2, Screen.height - height * 2,
					width, height), action.actionMessage);
			}
		}
	}

	void Update() {
		if (state == ActionState.PAUSED) {
			canUse = false;
			canDrop = false;
			canMelee = false;
			canSwitch = false;
			playerActor.canInteract = false;
			playerMove.canJump = false;
			playerMove.canMove = false;
			playerMove.canMoveHead = false;
			playerMove.canTurnBody = false;
		} else if (state == ActionState.FREE) {
			canUse = true;
			canDrop = true;
			canMelee = true;
			canSwitch = true;
			playerActor.canInteract = true;
			playerMove.canJump = true;
			playerMove.canMove = true;
			playerMove.canMoveHead = true;
			playerMove.canTurnBody = true;
		} else if (state == ActionState.ACTING) {
			canUse = action.canUseItems;
			canDrop = action.canDrop;
			canMelee = false;
			canSwitch = action.canSwitchItems;
			playerActor.canInteract = action.canInteract;
			playerMove.canJump = action.canJump;
			playerMove.canMove = action.canMove;
			playerMove.canMoveHead = action.canCameraTurn;
			playerMove.canTurnBody = action.canTurnBody;
		} else if (state == ActionState.DEAD) {
			canUse = false;
			canDrop = false;
			canMelee = false;
			canSwitch = false;
			playerActor.canInteract = false;
			playerMove.canJump = false;
			playerMove.canMove = false;
			playerMove.canMoveHead = false;
			playerMove.canTurnBody = false;
		} else if (state == ActionState.COOLDOWN) {
			cooldownRemain -= Time.deltaTime;
			if (cooldownRemain <= 0) {
				state = ActionState.FREE;
				canUse = true;
				canDrop = true;
				canMelee = true;
				canSwitch = true;
				playerActor.canInteract = true;
				playerMove.canJump = true;
				playerMove.canMove = true;
				playerMove.canMoveHead = true;
				playerMove.canTurnBody = true;
			} else {
				canUse = false;
				canDrop = true;
				canMelee = false;
				canSwitch = true;
				playerActor.canInteract = false;
				playerMove.canJump = true;
				playerMove.canMove = true;
				playerMove.canMoveHead = true;
				playerMove.canTurnBody = true;
			}
		}
		prevState = state;

		if (isLocalPlayer) {
			if (GetComponent<Damageable> ().IsDead ()) {
				state = ActionState.DEAD;

				GetComponent<CharacterController> ().enabled = false;

				animator.SetBool ("Dead", true);

				for (int i = 0; i < inventory.items.Length; i++) {
					inventory.DropItem(i);
				}
			}

			if (Input.GetButton ("Interact")) {
				playerActor.InteractWithObject ();
			}
			if (canDrop && Input.GetButton ("Drop") && inventory.IsHoldingItem ()) {
				inventory.DropItem (inventory.selected);
			}
			if (canUse && Input.GetButton ("Use") && inventory.IsHoldingItem () && !prevHeld) {
				CmdUse ();
			}
			if (canMelee && Input.GetButton ("Melee")) {
				attacker.CmdAttemptMeleeAttack ();
			}
			if (canSwitch) {
				if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					inventory.selected += 1;
					if (inventory.selected >= inventory.items.Length)
						inventory.selected = 0;
				} else if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					inventory.selected -= 1;
					if (inventory.selected < 0)
						inventory.selected = inventory.items.Length - 1;
				}
			}
			prevHeld = Input.GetButton ("Use");
		}
	}
}

