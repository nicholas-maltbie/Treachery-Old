using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Item : NetworkBehaviour {

	public Interactable clicker;

	public float mass = 1, drag = 0, angularDrag = 0.05f;
	public bool useGravity = true, isKinematic = true;
	[SyncVar]
	public string name, description, flavorText;
	[SyncVar]
	private GameObject holder;
	private float attempts;
	public bool hasPhysics;
	private bool isHeld, initialized;
	public Texture icon; 

	[Command]
	public void CmdAttemptPickup (GameObject actor) 
	{
		Inventory bag = actor.GetComponent<Inventory>();
		if (bag.HasOpenSpace ()) {
			holder = actor;
			if(GetComponent<NetworkIdentity> ().clientAuthorityOwner != actor.GetComponent<NetworkIdentity>().connectionToClient) {
				GetComponent<NetworkIdentity>().AssignClientAuthority(actor.GetComponent<NetworkIdentity>().connectionToClient);
			}
			bag.PickupItem(gameObject);
			gameObject.SendMessage("PickupItem");
			clicker.canInteract = false;
			isHeld = true;
			if(hasPhysics)
				DisablePhysics();
			GetComponent<NetworkTransform>().enabled = false;
			gameObject.layer = 0;
		}
	}

	public void Interact (GameObject actor)
	{
		CmdAttemptPickup (actor);
	}

	public void DisablePhysics()
	{
		if(GetComponent<Rigidbody> () != null)
			Destroy (GetComponent<Rigidbody> ());
	}

	[ServerCallback]
	public void EnablePhysics()
	{
		gameObject.AddComponent<Rigidbody> ();
		Rigidbody rigidbody = GetComponent<Rigidbody> ();
		rigidbody.mass = mass;
		rigidbody.drag = drag;
		rigidbody.angularDrag = angularDrag;
		rigidbody.useGravity = useGravity;
	}

	public void DropItem (GameObject player)
	{
		gameObject.SendMessage ("ItemDropped");
		holder = null;
		isHeld = false;
		clicker.canInteract = true;
		if(gameObject.GetComponent<NetworkIdentity> ().clientAuthorityOwner != player.GetComponent<NetworkIdentity>().connectionToClient)
			gameObject.GetComponent<NetworkIdentity> ().RemoveClientAuthority (player.GetComponent<NetworkIdentity>().connectionToClient);
		EnablePhysics ();
		GetComponent<NetworkTransform>().enabled = true;
	}

	public void ItemDropped()
	{
		//gameObject.layer = INTERACTABLE_LAYER;
	}

	public void PutInBag()
	{

	}

	public void HoldItem()
	{

	}

	public void PickupItem()
	{

	}

	public GameObject getHolder()
	{
		return holder;
	}

	public void Update()
	{
		if (!initialized) {
			if(holder != null && !isHeld)
			{
				//holder.GetComponent<Player>().PickupItem(gameObject);
				transform.parent = holder.transform;
				initialized = true;
			}
			attempts += Time.deltaTime;
			if(attempts > 0.1)
				initialized = true;
		}

		if (holder != null) {
			gameObject.transform.parent = holder.transform;
		} else if (holder == null) {
			gameObject.transform.parent = null;
		}
	}

	public void Start()
	{
		if (GetComponent<NetworkIdentity> ().isServer && hasPhysics) {
			EnablePhysics ();
		} else {
			DisablePhysics ();
		}
	}
}
