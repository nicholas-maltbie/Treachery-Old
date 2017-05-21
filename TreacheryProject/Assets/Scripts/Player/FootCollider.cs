using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script meant to inform the footstep sound generator when a foot hits the ground.
/// </summary>
public class FootCollider : MonoBehaviour {

	/// <summary>
	/// Minimum time between footsteps.
	/// </summary>
	public float minDelay = 0.1f;
	/// <summary>
	/// Object handling sounds
	/// </summary>
	public FootSounds soundHandler;
	/// <summary>
	/// Name for this foot
	/// </summary>
	public string footName;

	/// <summary>
	/// Was the foot grounded the previosu frame.
	/// </summary>
	private bool prevGround;
	/// <summary>
	/// Time since last footstep
	/// </summary>
	private float elapsed = 0.0f;

	void Update() {
		string material = null;
		RaycastHit hit;
		bool grounded = false;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity)) {
			grounded = hit.distance <= .1;
			if (! prevGround && grounded && elapsed >= minDelay) {
				material = hit.collider.material.name;
				material = material.Substring (0, material.IndexOf ("(Instance)")).Trim ();
				soundHandler.FootDown (gameObject.transform.position, footName, material);
				elapsed = 0;
			}
		}
		prevGround = grounded;
		elapsed += Time.deltaTime;
	}
}
