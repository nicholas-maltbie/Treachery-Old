using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Networking;

/**
 * Character foot sounds, sounds when the charactr's foot does stuff.
 * Supported events - 
 * 	Lift - When the player jumps
 *  Land - When the player lands
 *  FootDown - Whenever a foot hits the ground
 * 
 * Each of these sounds can have a different sound based on the material.
 */
public class FootSounds : NetworkBehaviour {
	public FootstepIndex soundIndex;
	public Transform basePos;
	public bool makeFootsteps = true;
	public float volumeMult = 1.0f;

	public void Lift() {
		if (makeFootsteps) {
			RaycastHit hit;
			if (Physics.Raycast (basePos.position, Vector3.down, out hit, Mathf.Infinity)) {
				string material = hit.collider.material.name;
				material = material.Substring (0, material.IndexOf ("(Instance)")).Trim ();

				if (soundIndex.materialSounds.ContainsKey (material)) {
					CmdMakeSound ("Jump", material, volumeMult, basePos.position);
				}
			}
		}
	}

	[Command]
	public void CmdMakeSound(string type, string material, float volumeMultiplier, Vector3 position) {
		RpcPlaySound (type, material, volumeMultiplier, position);
	}

	[ClientRpc]
	public void RpcPlaySound(string type, string material, float volumeMultiplier, Vector3 position) {
		List<AudioClip> clips = soundIndex.materialSounds [material] [type];
		AudioClip clip = clips [(int)Random.Range (0, clips.Count)];
		AudioSource.PlayClipAtPoint (clip, position, volumeMultiplier);
	}

	public void Land() {
		if (makeFootsteps) {
			RaycastHit hit;
			if (Physics.Raycast (basePos.position, Vector3.down, out hit, Mathf.Infinity)) {
				string material = hit.collider.material.name;
				material = material.Substring (0, material.IndexOf ("(Instance)")).Trim ();

				if (soundIndex.materialSounds.ContainsKey (material)) {
					CmdMakeSound ("Land", material, volumeMult, basePos.position);
				}
			}
		}

	}

	public void FootDown(Vector3 footPosition, string foot, string material) {
		if (makeFootsteps) {
			if (material == null || material == "Default") {
				material = soundIndex.defaultMat;
			}

			if (soundIndex.materialSounds.ContainsKey (material)) {
				CmdMakeSound ("Step", material, volumeMult, footPosition);
			}
		}
	}
}
