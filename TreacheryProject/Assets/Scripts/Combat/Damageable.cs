using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Damageable : NetworkBehaviour {

	public GameObject colliderBase;

	[SyncVar]
	public float maxHealth = 80, maxSanity = 80,
					health, sanity;

	// Use this for initialization
	void Start () {
		health = maxHealth;
		sanity = maxSanity;
		foreach (Collider col in colliderBase.GetComponentsInChildren<Collider>()) {
			DamageCollider damageThingy = col.gameObject.AddComponent<DamageCollider> ();
			damageThingy.damageable = this;
		}
	}

	[ServerCallback]
	public void DamageHealth(int amount) {
		SendMessage ("OnDamageHealth", amount);
		health -= amount;
	}

	[ServerCallback]
	public void DamageSanity(int amount) {
		SendMessage ("OnDamageSanity", amount);
		sanity -= amount;
	}

	[ServerCallback]
	public void HealHealth(int amount) {
		health = Mathf.Min (maxHealth, health + amount);
	}

	[ServerCallback]
	public void HealSanity(int amount) {
		sanity = Mathf.Min (maxSanity, sanity + amount);
	}

	public bool IsDead() {
		return sanity <= 0 || health <= 0;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
