using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour {

	public static HashSet<string> ignoreShaders = new HashSet<string>(new string[]{ "Outlined" });

	public float thickness = 0.05f;
	public Color outlineColor = Color.red;
	public bool shouldHighlight = false;
	private Material highlight = null;
	public List<Renderer> rens = new List<Renderer>();
	
	public void HighlightItem() {
		if (highlight == null) {
			highlight = new Material (Shader.Find ("Outlined/Silhouette Only"));
			highlight.SetFloat ("_Outline", thickness);
			highlight.SetColor ("_OutlineColor", outlineColor);
			highlight.shader.name = "Outlined";
			foreach (Renderer ren in rens) {
				Material[] mats = ren.materials;
				Material[] newMats = new Material[mats.Length + 1];
				for (int i = 0; i < mats.Length; i++) {
					newMats [i] = mats [i];
				}
				newMats [newMats.Length - 1] = highlight;
				ren.materials = newMats;
			}
		}
	}

	public void RemoveHighlight() {
		if (highlight != null) {
			foreach (Renderer ren in rens) {
				Material[] mats = ren.materials;
				List<Material> newMats = new List<Material> ();
				for (int i = 0; i < mats.Length; i++) {
					if (!ignoreShaders.Contains (mats [i].shader.name)) {
						newMats.Add (mats [i]);
					}
				}
				ren.materials = newMats.ToArray ();
			}
			highlight = null;
		}

	}

	void Start() {
		foreach (Renderer ren in GetComponentsInChildren<Renderer>()) {
			rens.Add (ren);
		}
	}

	void Update () {
		if (shouldHighlight) {
			HighlightItem ();
		} else {
			RemoveHighlight ();
		}

	}
}
