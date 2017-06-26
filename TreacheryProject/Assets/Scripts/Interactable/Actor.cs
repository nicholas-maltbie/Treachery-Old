using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// An actor interacts with interactable objects.
/// </summary>
public class Actor : NetworkBehaviour {

	/// <summary>
	/// The current object the actor is looking at.
	/// </summary>
	public Interactable interactable;
	/// <summary>
	/// The actor's viewpoint.
	/// </summary>
	public Transform actorCamera;
	/// <summary>
	/// Is this actor allowed to act.
	/// </summary>
	public bool canInteract;
	/// <summary>
	/// The amount that the actor can see.
	/// </summary>
	public float viewDistance = Mathf.Infinity;

	/// <summary>
	/// Is the actor currently looking at an object.
	/// </summary>
	/// <returns><c>true</c> if this actor is looking at an object; otherwise, <c>false</c>.</returns>
	public bool IsLooking() {
		return interactable != null;
	}

	/// <summary>
	/// Command to interact with the object.
	/// </summary>
	[Command]
	public void CmdInteract(GameObject looking) {
		looking.GetComponent<Interactable>().InteractWithObject (gameObject);
	}

	/// <summary>
	/// Send a command to the object to itneract with it.
	/// </summary>
	public void InteractWithObject() {
		if (IsLooking () && canInteract) {
			CmdInteract (interactable.gameObject);
		}
	}

	/// <summary>
	/// Update the status of the actor.
	/// </summary>
	void Update () {
		//Check with a sphere cast on the if the player si loking at something interactable
		RaycastHit hit;
		if (canInteract && Physics.SphereCast(actorCamera.position, .1f, 
			actorCamera.forward, out hit, Mathf.Infinity) && 
			hit.collider.gameObject.GetComponent<Interactable>() != null && 
			Mathf.Min(viewDistance, hit.distance) <= hit.collider.gameObject.GetComponent<Interactable>().useDistance)
		{
			this.interactable = hit.collider.gameObject.GetComponent<Interactable>();
		}
		else {
			this.interactable = null;
		}
	}
}
