using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Inventory))]
public class GraphicalInventory : NetworkBehaviour {

	public bool display;
	private Inventory inventory;

	private int gap = 10;
	private float width = 0.15f;

	// Use this for initialization
	void Start () {
		display = isLocalPlayer;
		inventory = GetComponent<Inventory> ();
	}

	void OnGUI() {
		if (display) {
			string items = "";
			for (int index = 0; index < inventory.items.Length; index++) {
				items += " " + (index + 1) + ".";
				if (inventory.selected == index)
					items += "*";
				else
					items += " ";
				items += " " + inventory.items [index];
				if (index < inventory.items.Length)
					items += "\n";
			}
			TextAnchor align = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.LowerLeft;
			Vector2 guiBox = GUI.skin.box.CalcSize (new GUIContent (items));
			GUI.Box (
				new Rect (Screen.width - Screen.width * width - gap, Screen.height - guiBox.y - gap, 
					Screen.width * width, guiBox.y), "");
			GUI.Label (
				new Rect (Screen.width - Screen.width * width - gap, Screen.height - guiBox.y - gap, 
					Screen.width * width, guiBox.y), 
				items);
			GUI.skin.label.alignment = align;
		}
	}
}
