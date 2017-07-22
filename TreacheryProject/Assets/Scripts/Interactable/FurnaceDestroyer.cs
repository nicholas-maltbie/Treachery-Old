using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FurnaceDestroyer : NetworkBehaviour {

	public enum FurnaceState {CLOSED, OPEN, CONSUMING};

	public float consumeTime = 60f;
	public Animator doorAnim;
	public FurnaceState state = FurnaceState.CLOSED;
	public Transform coal;

	private float transitionDelay = 0f;
	private GameObject consuming;
	private float currentConsume = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isServer) {
			if (state == FurnaceState.CONSUMING) {
				currentConsume += Time.deltaTime;
				if (currentConsume >= consumeTime) {
					Destroy (consuming);
					state = FurnaceState.OPEN;
					doorAnim.SetBool ("Open", true);
				}
			}
			if (transitionDelay > 0) {
				transitionDelay -= Time.deltaTime;
			}


		} else {
			currentConsume = 0;
		}
	}

	void Interact(GameObject actor) {
		if (transitionDelay <= 0) {
			if (state == FurnaceState.CLOSED) {
				doorAnim.SetBool ("Open", true);
				state = FurnaceState.OPEN;
				transitionDelay = 2f;
			} else if (state == FurnaceState.OPEN) {
				GamePlayer player = actor.GetComponent<GamePlayer> ();
				if (player != null) {
					Inventory inv = player.GetComponent<Inventory> ();
					GameObject held = inv.held;
					if (held != null) {
						inv.ServerDropItem (inv.selected);
						consuming = held;
						consuming.transform.position = coal.position;
						player.EndAction (1.75f, "You put the item into the furnace");
						consuming.GetComponent<Item> ().DisablePhysics ();
						state = FurnaceState.CONSUMING;
						doorAnim.SetBool ("Open", false);
						transitionDelay = 1f;
					} else {
						state = FurnaceState.CLOSED;
						doorAnim.SetBool ("Open", false);
					}
				} else {
					state = FurnaceState.CLOSED;
					doorAnim.SetBool ("Open", false);
				}
			} else if (state == FurnaceState.CONSUMING) {
				GamePlayer player = actor.GetComponent<GamePlayer> ();
				if (player != null) {
					Inventory inv = player.GetComponent<Inventory> ();
					if (inv.HasOpenSpace ()) {
						state = FurnaceState.OPEN;
						transitionDelay = 2f;
						doorAnim.SetBool ("Open", true);
						inv.AttemptPickup (consuming.GetComponent<Item> ());
						player.EndAction (2.5f, "You pull the item from the burning furnace");
					} else {
						player.EndAction (1.0f, "Open space to pick up the item quick");
					}
				}
			}
		} 
	}
}
