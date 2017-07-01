using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Actor))]
[RequireComponent(typeof (PlayerMove))]
[RequireComponent(typeof(Inventory))]
public class GamePlayer : NetworkBehaviour {
	private Actor playerActor;
	private PlayerMove playerMove;
	private Inventory inventory;
	public bool canSwitch;

	private bool prevHeld;

	void Start() {
		playerActor = GetComponent<Actor> ();
		playerMove = GetComponent<PlayerMove> ();
		inventory = GetComponent<Inventory> ();
	}

	[Command]
	public void CmdUse() {
		inventory.held.GetComponent<Item> ().ServerUse (gameObject);
	}

	void Update() {
		if (isLocalPlayer) {
			if (Input.GetButton ("Interact")) {
				playerActor.InteractWithObject ();
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
			if (Input.GetButton ("Drop") && inventory.IsHoldingItem()) {
				inventory.DropItem (inventory.selected);
			}

			if (Input.GetButton ("Use") && inventory.IsHoldingItem () && !prevHeld) {
				CmdUse ();
			}
			prevHeld = Input.GetButton ("Use");
		}
	}
}

