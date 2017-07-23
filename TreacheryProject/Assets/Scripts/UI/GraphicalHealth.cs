using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Damageable))]
public class GraphicalHealth : NetworkBehaviour {
	public bool display;
	private Damageable health;

	private int gap = 10;
	private float width = 0.15f;

	// Use this for initialization
	void Start () {
		display = isLocalPlayer;
		health = GetComponent<Damageable> ();
	}

	void OnGUI() {
		if (display) {
			string text = 
				"Health: " + health.health + "/" + health.maxHealth + "\n" +
				"Sanity: " + health.sanity + "/" + health.maxSanity;
			
			TextAnchor align = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.LowerLeft;
			Vector2 guiBox = GUI.skin.box.CalcSize (new GUIContent (text));
			GUI.Box (
				new Rect (gap, Screen.height - guiBox.y - gap, 
					Screen.width * width, guiBox.y), "");
			GUI.Label (
				new Rect (gap * 2, Screen.height - guiBox.y - gap, 
					Screen.width * width, guiBox.y), 
				text);
			GUI.skin.label.alignment = align;
		}
	}
}
