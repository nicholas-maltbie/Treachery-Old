using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// An interuptable action that a player can perform but can be interupted.
/// </summary>
/*abstract public class InteruptableAction : NetworkBehaviour {

	/// <summary>
	/// The elapsed time.
	/// </summary>
	public float elapsedTime;
	/// <summary>
	/// The total time it takes to perform this action.
	/// </summary>
	public float totalTime;
	/// <summary>
	/// The message.
	/// </summary>
	public string message = "Generic interuptable action examine message";
	/// <summary>
	/// The player who is performing this action.
	/// </summary>
	public Player performer;
	/// <summary>
	/// Has this action been interupted.
	/// </summary>
	public bool interupted;

	/// <summary>
	/// Interupts the action.
	/// </summary>
	public void InteruptAction()
	{
		if (IsInUse ()) {
			performer.gameObject.GetComponent<MultiplayerCharacterController> ().RpcSetMoveInteractExamine (true);
			interupted = true;
			performer.interuptableAction = null;
			performer.SetActionCooldown ("Action Interupted", 3);
			ActionInterupted ();
			performer = null;
		}
	}

	/// <summary>
	/// Determines whether this instance is in use.
	/// </summary>
	/// <returns><c>true</c> if this instance is in use; otherwise, <c>false</c>.</returns>
	public bool IsInUse()
	{
		return performer != null;
	}

	/// <summary>
	/// Performs the action.
	/// </summary>
	/// <param name="player">Player.</param>
	public void PerformAction(Player player)
	{
		performer = player;
		performer.interuptableAction = gameObject;
		performer.gameObject.GetComponent<MultiplayerCharacterController> ().RpcSetMoveInteractExamine (false);
		performer.RpcSetPercentMessage (true);
		performer.RpcSetPercentText(message);
		performer.RpcSetPercentComplete(0);
		elapsedTime = 0;
	}
	
	// Update is called once per frame
	public void Update () {
		if (isServer && performer != null) {
			performer.RpcSetPercentText (message);
			if (interupted) {
				interupted = false;
				elapsedTime = 0;
				return;
			}
			elapsedTime += Time.deltaTime;
			performer.RpcSetPercentComplete(elapsedTime / totalTime * 100);
			if (elapsedTime >= totalTime) {
				performer.gameObject.GetComponent<MultiplayerCharacterController> ().RpcSetMoveInteractExamine (true);
				ActionCompleted ();
				performer.interuptableAction = null;
				performer = null;
			}

		}
	}
	/// <summary>
	/// Called when the action has been completed.
	/// </summary>
	abstract public void ActionCompleted();
	/// <summary>
	/// Called when the action is Interupted.
	/// </summary>
	abstract public void ActionInterupted();

}
*/