using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TargetDummy : NetworkBehaviour {

	public Animator animator;

	private float shakeTime = 1f;
	private float shakeElapsed = 0f;
	private float respawnTime = 1.5f;
	private float respawnElapsed = 0f;

	void OnDamageHealth(int amount) {
		if (shakeElapsed == 0) {
			SetShakeState(true);
		}
	}
	
	public void SetShakeState(bool state) {
		animator.SetBool ("Shake", state);
	}

	void OnDamageSanity(int amount) {
		if (shakeElapsed == 0) {
			SetShakeState(true);
		}
	}

	void Update() {
		if (isServer) {
			if (animator.GetBool ("Shake")) {
				shakeElapsed += Time.deltaTime;
				if (shakeElapsed >= shakeTime) {
					shakeElapsed = 0;
					SetShakeState (false);
				}
			}

			if (GetComponent<Damageable> ().IsDead () || animator.GetBool("Die")) {
				animator.SetBool ("Die", true);
				GetComponent<Damageable> ().canBeAttacked = false;
				respawnElapsed += Time.deltaTime;
				if (respawnElapsed >= respawnTime) {
					GetComponent<Damageable> ().SetHealth ((int) GetComponent<Damageable> ().maxHealth );
					GetComponent<Damageable> ().canBeAttacked = true;
					animator.SetBool ("Die", false);
				}
			} else {
				respawnElapsed = 0;
				animator.SetBool ("Die", false);
			}
		}
	}
}
