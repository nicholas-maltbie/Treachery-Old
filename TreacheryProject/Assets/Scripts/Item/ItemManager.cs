using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour {

	public Item[] possibleItems;
	private static Dictionary<string, GameObject> indexedItems = new Dictionary<string, GameObject>();

	void Start() {
		foreach (Item item in possibleItems)
			indexedItems.Add (item.itemName, item.gameObject);
		if (isServer) {
			foreach (Item item in GameObject.FindObjectsOfType<Item>())
				item.EnablePhysics ();
		}
	}

	[ServerCallback]
	public static GameObject SpawnItem(string name) {
		if (indexedItems.ContainsKey (name)) {
			GameObject prefab = indexedItems [name];
			GameObject spawned = Instantiate (prefab);
			spawned.GetComponent<Item>().EnablePhysics ();
			NetworkServer.Spawn (spawned);
			return spawned;
		}
		return null;
	}

	[ServerCallback]
	public static GameObject SpawnHeldItem(string name) {
		GameObject item = SpawnItem (name);
		item.GetComponent<Item> ().isHeld = true;
		foreach (Interactable interactable in item.GetComponentsInChildren<Interactable>()) {
			interactable.enabled = false;
		}
		return item;
	}
}
