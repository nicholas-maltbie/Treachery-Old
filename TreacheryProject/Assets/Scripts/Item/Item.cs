using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Item : NetworkBehaviour {
	
	public string itemName;
	public Transform holdPos;
	[SyncVar]
	public bool isHeld;

	[ServerCallback]
	public void ServerUse(GameObject holder) {
		gameObject.SendMessage ("Use", holder);
	}

	public void Use(GameObject holder) {
		// Do something
	}

	public void Interact (GameObject actor)
	{
		if (!isHeld) {
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
			GetComponent<NetworkTransform> ().transformSyncMode = NetworkTransform.TransformSyncMode.SyncNone;
		}
	}

	public void EnablePhysics()
	{
		GetComponent<Rigidbody> ().useGravity = true;
		GetComponent<Rigidbody> ().detectCollisions = true;
		gameObject.GetComponent<NetworkTransform> ().transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody3D;
	}
}
