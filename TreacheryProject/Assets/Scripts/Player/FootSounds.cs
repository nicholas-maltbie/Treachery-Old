using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

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
	public FootstepIndex soundIndex;
	public Transform basePos;
	public bool makeFootsteps = true;
	public float volumeMult = 1.0f;

	void Lift() {
		if (makeFootsteps) {
			RaycastHit hit;
			if (Physics.Raycast (basePos.position, Vector3.down, out hit, Mathf.Infinity)) {
				string material = hit.collider.material.name;
				material = material.Substring (0, material.IndexOf ("(Instance)")).Trim ();

				if (soundIndex.materialSounds.ContainsKey (material)) {
					List<AudioClip> clips = soundIndex.materialSounds [material] ["Jump"];
					AudioClip clip = clips [(int)Random.Range (0, clips.Count)];

					AudioSource.PlayClipAtPoint (clip, basePos.position, volumeMult);
				}
			}
		}
	}

	void Land() {

	}

	public void FootDown(Vector3 footPosition, string foot, string material) {
		if (makeFootsteps) {
			if (material == null || material == "Default") {
				material = soundIndex.defaultMat;
			}

			if (soundIndex.materialSounds.ContainsKey (material)) {
				List<AudioClip> clips = soundIndex.materialSounds [material] ["Step"];
				AudioClip clip = clips [(int)Random.Range (0, clips.Count)];

				AudioSource.PlayClipAtPoint (clip, footPosition, volumeMult);
			}
		}
	}
}
