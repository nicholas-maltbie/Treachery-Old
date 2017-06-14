using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// A description attached to an object and allow players to view information about the object.
/// </summary>
public class Description : NetworkBehaviour {
	/// <summary>
	/// The search icon.
	/// </summary>
	public Texture searchIcon;
	/// <summary>
	/// The delay after the player searches the object before any other action.
	/// </summary>
	public float delay = 3f;
	/// <summary>
	/// The name of the object.
	/// </summary>
	public string objectName = "Examinable object";
	/// <summary>
	/// The description.
	/// </summary>
	public string description = "Generic description for object in the house";
	
	void Start () {
		GetComponent<Interactable>().interactionMessage = "Examine";
		GetComponent<Interactable>().icons.Add(searchIcon);
		GetComponent<Interactable>().messages.Add("Examine");
	}

	void Update () {
		
	}

	/// <summary>
	/// When a player interacts with this object.
	/// </summary>
	/// <param name="player">Player.</param>
	[ServerCallback]
	void Interact(GameObject player)
	{
		GetComponent<Interactable>().canInteract = true;
		//player.GetComponent<Player> ().RpcSetActionCooldown (objectName + ": " + description, delay);
	}
}
