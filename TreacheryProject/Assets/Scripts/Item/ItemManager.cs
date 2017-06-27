using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour {

	public GameObject[] possibleItems;
	private static Dictionary<string, GameObject> indexedItems = new Dictionary<string, GameObject>();

	void Start() {
		foreach (GameObject item in possibleItems)
			indexedItems.Add (item.GetComponent<Item>().itemName, item);
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
	
	public static GameObject SpawnHeldItem(string name) {
		if (indexedItems.ContainsKey (name)) {
			GameObject prefab = indexedItems [name];
			GameObject item = Instantiate (prefab);
			if (item.GetComponent<NetworkIdentity> () != null) {
				item.GetComponent<NetworkIdentity> ().enabled = false;
			}
			item.GetComponent<Item> ().isHeld = true;
			foreach (Interactable interactable in item.GetComponentsInChildren<Interactable>()) {
				interactable.enabled = false;
			}
			return item;
		}
		return null;
	}
}
