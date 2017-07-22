using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Invisible : NetworkBehaviour {
	
	public static HashSet<string> ignoreShaders = new HashSet<string>(new string[]{ "Outlined/Silhouette Only" });

	[SyncVar]
	public bool invisibleState;
	private bool currState;
	public List<Renderer> rens = new List<Renderer>();
	public Dictionary<Renderer, List<Material>> objectMats = null;

	[ServerCallback]
	public void SetState(bool state) {
		invisibleState = state;
	}

	private void MakeInvisible() {
		currState = true;
		if (objectMats == null) {
			objectMats = new Dictionary<Renderer, List<Material>> ();
			foreach (Renderer ren in rens) {
				List<Material> keep = new List<Material> ();
				Material[] mats = ren.materials;
				objectMats[ren] = new List<Material> ();
				for (int i = 0; i < mats.Length; i++) {
					if (ignoreShaders.Contains (mats [i].shader.name)) {
						keep.Add (mats [i]);
					} else {
						objectMats [ren].Add (mats [i]);
					}
				}
				ren.materials = keep.ToArray ();
			}
		}
	}
	
	private void MakeVisible() {
		currState = false;
		if (objectMats != null) {
			foreach (Renderer ren in rens) {
				List<Material> final = new List<Material> (objectMats[ren]);
				final.AddRange (ren.materials);
				ren.materials = final.ToArray();
			}
			objectMats = null;
		}

	}

	void Start() {
		foreach (Renderer ren in GetComponentsInChildren<Renderer>()) {
			rens.Add (ren);
		}
	}

	// Update is called once per frame
	void Update () {
		if (invisibleState && !currState) {
			MakeInvisible ();
		} else if (!invisibleState && currState) {
			MakeVisible ();
		}
	}
}
