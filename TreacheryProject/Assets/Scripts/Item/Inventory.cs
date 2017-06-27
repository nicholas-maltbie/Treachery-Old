using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Inventory : NetworkBehaviour {

	public int selected;
	public string[] items;
	public GameObject held;
	public Transform hand;

	public void Update() {
		if (isLocalPlayer) {
			if (held == null) { //not holding an item
				if (IsSpotEmpty (selected)) {
					//Do nothing
				} else {		//Supposed to be holding an item
					CmdHoldItem (items [selected]);
				}
			} else {			//holding an item
				if (IsSpotEmpty (selected)) {
					CmdRemoveHeldItem ();
				} else {		//Supposed to be holding an item
					if (held.GetComponent<Item> ().itemName == items [selected]) {
						//Do nothing, this is good
					} else {
						CmdRemoveHeldItem ();
						CmdHoldItem (items [selected]);
					}
				}
			}
		}
	}

	[Command]
	public void CmdRemoveHeldItem() {
		RpcRemoveHeldItem ();
	}

	[Command]
	public void CmdDropItem(int index)
	{
		GameObject spawned = ItemManager.SpawnItem (items [index]);
		spawned.transform.position = held.transform.position;
		spawned.transform.rotation = held.transform.rotation;
		RpcRemoveHeldItem ();
		RpcEmptyItemSlot (index);
	}

	[Command]
	public void CmdHoldItem(string itemName) {
		RpcRemoveHeldItem ();
		RpcPutItemInHand (itemName);
	}

	[ClientRpc]
	public void RpcPutItemInHand(string itemName) {
		GameObject item = ItemManager.SpawnHeldItem (itemName);
		item.GetComponent<Item> ().DisablePhysics ();
		item.transform.position = hand.position;
		item.transform.rotation = hand.rotation;
		item.transform.parent = hand;
		held = item;
	}

	[ClientRpc]
	public void RpcRemoveHeldItem() {
		if (held != null) {
			Destroy (held);
			held = null;
		}
	}

	[ServerCallback]
	public void PickupItem(GameObject item) {
		NetworkServer.Destroy (item);
	}

	[ServerCallback]
	public void AttemptPickup(Item item) {
		if (HasOpenSpace ()) {
			RpcAddItemToHand (item.itemName, FirstOpenSpace ());
			items [FirstOpenSpace ()] = item.itemName;
			item.isHeld = true;
			PickupItem (item.gameObject);
		}
	}

	[ClientRpc]
	public void RpcAddItemToHand(string item, int space) {
		items [space] = item;
	}

	[ClientRpc]
	public void RpcEmptyItemSlot(int space) {
		items [space] = "";
	}

	public bool IsSpotEmpty(int spot) {
		return items [spot] == null || items [spot] == "";
	}

	public int FirstOpenSpace() {
		for (int i = 0; i < items.Length; i++)
			if (IsSpotEmpty(i))
				return i;
		return -1;
	}

	public bool IsHoldingItem() {
		return ! IsSpotEmpty (selected);
	}

	public bool HasOpenSpace()
	{
		return FirstOpenSpace() != -1;
	}
}
