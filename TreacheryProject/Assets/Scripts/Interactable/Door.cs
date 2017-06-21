using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Door : NetworkBehaviour {

	public Animator doorAnimator;
	public Interactable[] handles;

	[ClientRpc]
	public void RpcUpdateInteractState(bool state) {
		foreach (Interactable handle in handles) {
			handle.canInteract = state;
		}
	}

	[ServerCallback]
	public void Open() {
		RpcUpdateInteractState (true);
	}

	[ServerCallback]
	public void Close() {
		RpcUpdateInteractState (true);
	}

	// Update is called once per frame
	void Interact (GameObject actor) {
		doorAnimator.SetBool ("open", !doorAnimator.GetBool ("open"));
		RpcUpdateInteractState (false);
	}
}
