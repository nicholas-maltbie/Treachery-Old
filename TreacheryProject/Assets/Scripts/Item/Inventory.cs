using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Inventory : NetworkBehaviour {

	[SyncVar]
	public int selected;
	public GameObject[] items;
	[SyncVar]
	public GameObject held;
	public Transform hand;

	private GameObject inactiveItems;

	void Start() {
		inactiveItems = new GameObject ();
		inactiveItems.transform.parent = gameObject.transform;
		inactiveItems.SetActive (false);
		inactiveItems.name = "InactiveItems";
	}

	public void DropItem(int index) {
		if (!isServer) {
			GameObject dropped = items [index];
			dropped.transform.position = hand.transform.position;
			dropped.transform.rotation = hand.transform.rotation;
			dropped.transform.parent = null;
			items [index] = null;
			if (index == selected)
				held = null;
			dropped.GetComponent<Item> ().isHeld = false;
		}
		CmdDropItem (index);
	}

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
					CmdHideItem (held);
					held = null;
				} else {		//Supposed to be holding an item
					if (held == items [selected]) {
						//Do nothing, this is good
					} else {
						CmdHideItem (held);
						CmdHoldItem (items [selected]);
					}
				}
			}
		}
	}

	[Command]
	public void CmdDropItem(int index)
	{
		GameObject dropped = items [index];
		if (dropped != null) {
			items [index] = null;
			if (index == selected) {
				held = null;
			}
			dropped.transform.position = hand.transform.position;
			dropped.transform.rotation = hand.transform.rotation;
			dropped.transform.parent = null;
			RpcDropItem (index, dropped);
			dropped.GetComponent<Item> ().isHeld = false;
		}
	}

	[Command]
	public void CmdHoldItem(GameObject item) {
		CmdHideItem (held);
		RpcPutItemInHand (item);
	}

	[ClientRpc]
	public void RpcPutItemInHand(GameObject item) {
		if (item != null && item.GetComponent<Item> () != null) {
			item.GetComponent<Item> ().DisablePhysics ();
			item.transform.position = hand.position;
			item.transform.rotation = hand.rotation;
			item.transform.parent = hand;
			held = item;
		}
	}

	[ClientRpc]
	public void RpcDropItem (int index, GameObject dropped) {
		if (dropped != null) {
			dropped.transform.parent = null;
			dropped.GetComponent<Item> ().EnablePhysics ();
			items [index] = null;
		}
	}

	[Command]
	public void CmdHideItem(GameObject item) {
		RpcHideItem (item);
	}

	[ClientRpc]
	public void RpcHideItem(GameObject item) {
		if (item != null) {
			item.transform.parent = inactiveItems.transform;
			item = null;
		}
	}

	[ServerCallback]
	public void PickupItem(GameObject item) {
		RpcHideItem (item);
	}

	[ServerCallback]
	public void AttemptPickup(Item item) {
		if (HasOpenSpace ()) {
			int space = FirstOpenSpace ();
			items [space] = item.gameObject;
			RpcAddItemToHand (item.gameObject, space);
			item.isHeld = true;
			PickupItem (item.gameObject);
		}
	}

	[ClientRpc]
	public void RpcAddItemToHand(GameObject item, int space) {
		items [space] = item;
	}

	[ClientRpc]
	public void RpcEmptyItemSlot(int space) {
		items [space] = null;
	}

	public bool IsSpotEmpty(int spot) {
		return items [spot] == null;
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
