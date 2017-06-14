using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// An object that the player can interact with.
/// </summary>
public class Interactable : MonoBehaviour {
	/// <summary>
	/// The icons.
	/// </summary>
	public List<Texture> icons = new List<Texture>();
	/// <summary>
	/// The messages that correspond with the respective icons.
	/// </summary>
	public List<string> messages = new List<string>();

	/// <summary>
	/// The distace that a player must be within in order to interact with this object (in unity units).
	/// </summary>
	public float useDistance = 2;
	/// <summary>
	/// The script that is called when the object is interacted with.
	/// </summary>
	public GameObject actionScript;
	/// <summary>
	/// The current interaction message to determine the icon.
	/// </summary>
	public string interactionMessage;
	/// <summary>
	/// Can this object be interacted with.
	/// </summary>
	public bool canInteract = true;

	/// <summary>
	/// Interacts the with object.
	/// </summary>
	/// <param name="player">Player who is giving the command.
	/// </param>
	public void InteractWithObject (GameObject actor)
	{
		if (canInteract) {
			actionScript.SendMessage ("Interact", actor);
		}
	}

	/// <summary>
	/// Gets the current icon.
	/// </summary>
	/// <returns>The icon or null if the current interaction message has no corresponding icon.</returns>
	public Texture GetIcon()
	{
		if (HasIcon()) {
			return icons[messages.IndexOf (interactionMessage)];
		}
		return null;
	}

	/// <summary>
	/// Determines whether this instance has icon for its current message.
	/// </summary>
	/// <returns><c>true</c> if this instance has icon; otherwise, <c>false</c>.</returns>
	public bool HasIcon()
	{
		return messages.Contains(interactionMessage);
	}
}
