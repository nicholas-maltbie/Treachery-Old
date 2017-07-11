using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Actor))]
[RequireComponent(typeof (PlayerMove))]
[RequireComponent(typeof(Inventory))]
public class GamePlayer : NetworkBehaviour {
	public enum ActionState {FREE, ACTING, COOLDOWN, PAUSED};
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
		}
	}

	public PlayerType playerState = PlayerType.EXPLORER;
	private Actor playerActor;
	private PlayerMove playerMove;
	private Inventory inventory;
	public bool canSwitch = true, canDrop = true, canUse = true;
	public float cooldownRemain;
	
	private ActionState state = ActionState.FREE;
	private Action action;
	private bool prevHeld;

	private Haunt display;

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
	public bool InterruptAction(Action action) {
		if (action.canBeInterrupted) {
			action.controller.SendMessage ("Interrupt");
			return true;
		}
		return false;
	}

	[ServerCallback]
	public void SetAction(Action action) {
		this.state = ActionState.ACTING;
		this.action = action;
		RpcSetAction (action);
	}

	[ServerCallback]
	public void EndAction(float cooldown) {
		this.state = ActionState.COOLDOWN;
		this.cooldownRemain = cooldown;
		RpcEndAction(cooldown);
	}

	[ClientRpc]
	public void RpcSetAction(Action action) {
		this.state = ActionState.ACTING;
		this.action = action;
	}

	[ClientRpc]
	public void RpcEndAction(float cooldown) {
		this.state = ActionState.COOLDOWN;
		this.cooldownRemain = cooldown;
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
		if (display != null) {
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
		}
	}

	void Update() {
		if (state == ActionState.PAUSED) {
			canUse = false;
			canDrop = false;
			canSwitch = false;
			playerActor.canInteract = false;
			playerMove.canJump = false;
			playerMove.canMove = false;
			playerMove.canMoveHead = false;
			playerMove.canTurnBody = false;
		} else if (state == ActionState.FREE) {
			canUse = true;
			canDrop = true;
			canSwitch = true;
			playerActor.canInteract = true;
			playerMove.canJump = true;
			playerMove.canMove = true;
			playerMove.canMoveHead = true;
			playerMove.canTurnBody = true;
		} else if (state == ActionState.ACTING) {
			canUse = action.canUseItems;
			canDrop = action.canDrop;
			canSwitch = action.canSwitchItems;
			playerActor.canInteract = action.canInteract;
			playerMove.canJump = action.canJump;
			playerMove.canMove = action.canMove;
			playerMove.canMoveHead = action.canCameraTurn;
			playerMove.canTurnBody = action.canTurnBody;
		} else if (state == ActionState.COOLDOWN) {
			cooldownRemain -= Time.deltaTime;
			if (cooldownRemain <= 0) {
				state = ActionState.FREE;
				canUse = true;
				canDrop = true;
				canSwitch = true;
				playerActor.canInteract = true;
				playerMove.canJump = true;
				playerMove.canMove = true;
				playerMove.canMoveHead = true;
				playerMove.canTurnBody = true;
			} else {
				canUse = false;
				canDrop = true;
				canSwitch = true;
				playerActor.canInteract = false;
				playerMove.canJump = true;
				playerMove.canMove = true;
				playerMove.canMoveHead = true;
				playerMove.canTurnBody = true;
			}
		}

		if (isLocalPlayer) {
			if (Input.GetButton ("Interact")) {
				playerActor.InteractWithObject ();
			}
			if (canDrop && Input.GetButton ("Drop") && inventory.IsHoldingItem ()) {
				inventory.DropItem (inventory.selected);
			}
			if (canUse && Input.GetButton ("Use") && inventory.IsHoldingItem () && !prevHeld) {
				CmdUse ();
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

