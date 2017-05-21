using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Simple script meant to inform the footstep sound 
 * generator when a foot hits the ground
 */
public class FootCollider : MonoBehaviour {

	/**
	 * Minimum time between footsteps
	 */
	public float minDelay = 0.1f;
	/**
	 * Object handling sounds
	 */
	public FootSounds soundHandler;
	/**
	 * Name for this foot
	 */
	public string footName;

	/**
	 * Time since last footstep
	 */
	private float elapsed = 0.0f;

	void OnTriggerEnter(Collider other) {
		if (elapsed >= minDelay) {
			string material = null;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity)) {
				material = hit.collider.material.name;
				material = material.Substring (0, material.IndexOf ("(Instance)")).Trim();
			}
			soundHandler.FootDown (gameObject.transform.position, footName, material);
			elapsed = 0;
		}
	}

	void Update() {
		elapsed += Time.deltaTime;
	}
}
