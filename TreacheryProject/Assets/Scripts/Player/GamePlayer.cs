using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Actor))]
[RequireComponent(typeof (PlayerMove))]
public class GamePlayer : NetworkBehaviour {
	private Actor playerActor;
	private PlayerMove playerMove;

	void Start() {
		playerActor = GetComponent<Actor> ();
		playerMove = GetComponent<PlayerMove> ();
	}

	void Update() {
		if (isLocalPlayer) {
			if (Input.GetButton ("Interact")) {
				playerActor.InteractWithObject ();
			}
		}
	}
}

