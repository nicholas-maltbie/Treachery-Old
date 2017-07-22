using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Item : NetworkBehaviour {
	
	public string itemName;
	public Transform holdPos;
	[SyncVar]
	public bool isHeld;
	[SyncVar]
	public GameObject holder;
	[SyncVar]
	public bool canDrop = true;
	[SyncVar]
	public bool canPickup = true;

	void Update() {
		if (isHeld) {
			//GetComponent<NetworkTransform> ().transformSyncMode = NetworkTransform.TransformSyncMode.SyncNone;
			if (holder != null) {
				if (holder.GetComponent<Inventory> ().held == gameObject) {
					holder.GetComponent<Inventory> ().PutItemInHand (gameObject);
				} else {
					holder.GetComponent<Inventory> ().HideItem (gameObject);
				}
			}
		} else {
			//GetComponent<NetworkTransform> ().transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody3D;
		}
	}

	[ServerCallback]
	public void ServerUse(GameObject holder) {
		gameObject.SendMessage ("Use", holder);
	}

	public void Use(GameObject holder) {
		// Do something
	}

	public void Interact (GameObject actor)
	{
		if (!isHeld && canPickup) {
			Inventory bag = actor.GetComponent<Inventory> ();
			if (bag != null) {
				bag.AttemptPickup (this);
			}
		}
	}

	public void DisablePhysics()
	{
		if (GetComponent<Rigidbody> () != null) {
			GetComponent<Rigidbody> ().useGravity = false;
			GetComponent<Rigidbody> ().detectCollisions = false;
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			//GetComponent<NetworkTransform> ().transformSyncMode = NetworkTransform.TransformSyncMode.SyncNone;
		}
	}

	public void EnablePhysics()
	{
		GetComponent<Rigidbody> ().useGravity = true;
		GetComponent<Rigidbody> ().detectCollisions = true;
		//gameObject.GetComponent<NetworkTransform> ().transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody3D;
	}
}
