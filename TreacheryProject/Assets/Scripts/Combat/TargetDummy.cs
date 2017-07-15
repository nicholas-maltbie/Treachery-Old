using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TargetDummy : NetworkBehaviour {

	private float shakeTime = 2f;
	private float shakeElapsed = 0f;

	void OnDamageHealth(int amount) {
		if (shakeElapsed == 0) {
			GetComponent<Animator> ().SetBool ("Shake", true);
		}
	}

	void OnDamageSanity(int amount) {
		if (shakeElapsed == 0) {
			GetComponent<Animator> ().SetBool ("Shake", true);
		}
	}

	void Update() {
		if (isServer && GetComponent<Animator> ().GetBool ("Shake")) {
			shakeElapsed += Time.deltaTime;
			if (shakeElapsed >= shakeTime) {
				shakeElapsed = 0;
				GetComponent<Animator> ().SetBool ("Shake", false);
			}
		}
	}
}
