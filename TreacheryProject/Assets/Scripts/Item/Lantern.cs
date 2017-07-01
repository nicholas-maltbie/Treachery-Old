using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Lantern : NetworkBehaviour {

	[SyncVar]
	public bool state = true;
	public Light[] lights;

	public void Use(GameObject holder) {
		state = !state;
	}

	void Update() {
		foreach (Light l in lights) {
			l.enabled = state;
		}
	}
}
