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
					RpcRemoveHeldItem ();
				} else {		//Supposed to be holding an item
					if (held.GetComponent<Item> ().itemName == items [selected]) {
						//Do nothing, this is good
					} else {
						RpcRemoveHeldItem ();
						CmdHoldItem (items [selected]);
					}
				}
			}
		}
	}

	public void DropItem(int index)
	{
		items [index] = null;
	}

	[Command]
	public void CmdHoldItem(string itemName) {
		RpcRemoveHeldItem ();
		RpcPutItemInHand (ItemManager.SpawnHeldItem (itemName));
	}

	[ClientRpc]
	public void RpcPutItemInHand(GameObject item) {
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

	[Command]
	public void CmdPickupItem(GameObject item) {
		NetworkServer.Destroy (item);
	}

	public void AttemptPickup(Item item) {
		if (HasOpenSpace ()) {
			items [FirstOpenSpace ()] = item.itemName;
			item.isHeld = true;
			CmdPickupItem (item.gameObject);
		}
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
