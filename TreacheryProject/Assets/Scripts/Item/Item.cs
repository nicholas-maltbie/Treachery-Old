using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Item : Interactable {

	public const int INTERACTABLE_LAYER = 8;
	public float mass = 1, drag = 0, angularDrag = 0.05f;
	public bool useGravity = true, isKinematic = true;
	[SyncVar]
	public string name, description, flavorText;
	[SyncVar]
	private GameObject holder;
	private GameObject delayedHolder;
	private float attempts;
	public bool hasPhysics;
	private bool isHeld, initialized;
	public Texture icon; 

	[ServerCallback]
	public void Interact (GameObject player)
	{
		/*Player personThingy = player.GetComponent<Player>();
		if (personThingy.HasOpenSpace ()) {
			holder = player;
			if(gameObject.GetComponent<NetworkIdentity> ().clientAuthorityOwner != this.connectionToClient) {
				gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
			}
			personThingy.RpcPickupItem(gameObject);
			gameObject.SendMessage("PickupItem");
			canInteract = false;
			isHeld = true;
			if(hasPhysics)
				DisablePhysics();
			GetComponent<NetworkTransform>().enabled = false;
			gameObject.layer = 0;
		}*/
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
		rigidbody.isKinematic = isKinematic;
	}

	public void DropItem (GameObject player)
	{
		gameObject.SendMessage ("ItemDropped");
		holder = null;
		isHeld = false;
		canInteract = true;
		if(gameObject.GetComponent<NetworkIdentity> ().clientAuthorityOwner != this.connectionToClient)
			gameObject.GetComponent<NetworkIdentity> ().RemoveClientAuthority (player.GetComponent<NetworkIdentity>().connectionToClient);
		EnablePhysics ();
		GetComponent<NetworkTransform>().enabled = true;
	}

	public void ItemDropped()
	{
		gameObject.layer = INTERACTABLE_LAYER;
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

	/*public Player getHolder()
	{
		return holder.GetComponent<Player> ();
	}*/

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
		if (isServer && hasPhysics) {
			EnablePhysics();
		}
	}
}
