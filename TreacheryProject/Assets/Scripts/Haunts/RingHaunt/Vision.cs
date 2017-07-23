using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Vision : NetworkBehaviour {

	private float viewDist = 10f;
	private float cooldownTime = 8f;
	private float viewTime = 8f;
	private float elapsed = 0;
	private VisionState state = VisionState.READY;

	private GameObject user;

	public enum VisionState {READY, VISION, COOLDOWN};
	public bool highlightTraitor = true;

	public void UseSpecial1(GameObject user) {
		if (HauntManager.gameState == HauntManager.HauntState.HAUNT) {
			GamePlayer player = user.GetComponent<GamePlayer> ();

			if (state == VisionState.READY) {
				Item ring = RingHaunt.theRing.GetComponent<Item>();
				if (ring != null) {
					this.user = user;
					if (ring.isHeld) {
						if (RingHaunt.traitorHasRing) {
							elapsed = 0;
							float dist = Vector3.Distance (user.transform.position, RingHaunt.chosenOne.transform.position);
							if (dist <= viewDist) {
								highlightTraitor = true;
								RpcHighlightPlayer (RingHaunt.chosenOne.gameObject, user);
								player.EndAction (2.0f, "You see the traitor");
								player.AddTimedMessage ("Vision of traitor active", viewTime);
								state = VisionState.VISION;
							} else {
								player.EndAction (2.0f, "The traitor is too far away to see");
								user.GetComponent<GamePlayer> ().AddTimedMessage ("Vision on Cooldown", cooldownTime);
								state = VisionState.COOLDOWN;
							}
						} else {
							player.EndAction (2.0f, "An ally has the ring");
							user.GetComponent<GamePlayer> ().AddTimedMessage ("Vision on Cooldown", cooldownTime);
							state = VisionState.COOLDOWN;
						}
					} else {
						highlightTraitor = false;
						RpcHighlightRing (RingHaunt.theRing, user);
						player.EndAction (2.0f, "You see the ring");
						player.AddTimedMessage ("Vision of ring active", viewTime);
						state = VisionState.VISION;
					}
				}
			} else if (state == VisionState.COOLDOWN) {
				player.EndAction (0.5f, "Vision on cooldown");
			}
		}
	}

	[ClientRpc]
	public void RpcHighlightRing(GameObject ring, GameObject user) {
		if (user != null && user.GetComponent<GamePlayer> ().isLocalPlayer) {
			ring.GetComponent<Highlighter> ().HighlightItem ();
		}
	}

	[ClientRpc]
	public void RpcDeHighlightRing(GameObject ring, GameObject user) {
		if (user != null && user.GetComponent<GamePlayer> ().isLocalPlayer) {
			ring.GetComponent<Highlighter> ().RemoveHighlight ();
		}
	}

	[ClientRpc]
	public void RpcHighlightPlayer(GameObject target, GameObject user) {
		if (user != null && user.GetComponent<GamePlayer> ().isLocalPlayer) {
			target.GetComponent<GamePlayer> ().playerModel.GetComponent<Highlighter> ().HighlightItem ();
		}
	}

	[ClientRpc]
	public void RpcDeHighlightPlayer(GameObject target, GameObject user) {
		if (user != null && user.GetComponent<GamePlayer> ().isLocalPlayer) {
			target.GetComponent<GamePlayer> ().playerModel.GetComponent<Highlighter> ().RemoveHighlight ();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isServer) {
			if (state == VisionState.COOLDOWN) {
				elapsed += Time.deltaTime;
				if (elapsed >= cooldownTime) {
					elapsed = 0;
					state = VisionState.READY;
				}
			} else if (state == VisionState.VISION) {
				elapsed += Time.deltaTime;
				if (highlightTraitor) {
					RpcHighlightPlayer (RingHaunt.chosenOne.gameObject, user);
				} else {
					RpcHighlightRing (RingHaunt.theRing, user);
				}

				if (elapsed >= viewTime) {
					elapsed = 0;
					state = VisionState.COOLDOWN;
					user.GetComponent<GamePlayer> ().AddTimedMessage ("Vision on Cooldown", cooldownTime);
					if (highlightTraitor) {
						RpcDeHighlightPlayer (RingHaunt.chosenOne.gameObject, user);
					} else {
						RpcDeHighlightRing (RingHaunt.theRing, user);
					}
				}
			}
		}
	}
}
