using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Character foot sounds, sounds when the charactr's foot does stuff.
 * Supported events - 
 * 	Lift - When the player jumps
 *  Land - When the player lands
 *  FootDown - Whenever a foot hits the ground
 * 
 * Each of these sounds can have a different sound based on the material.
 */
public class FootSounds : MonoBehaviour {

	public Dictionary<string, List<AudioClip>> materialSounds;

	void Lift() {

	}

	void Land() {

	}

	public void FootDown(Vector3 footPosition) {

	}
}
