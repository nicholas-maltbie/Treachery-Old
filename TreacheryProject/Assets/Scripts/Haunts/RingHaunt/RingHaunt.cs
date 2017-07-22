using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class RingHaunt : Haunt {

	public static GamePlayer chosenOne = null;
	public static GameObject theRing = null;
	[SyncVar]
	public GamePlayer.PlayerType winner = GamePlayer.PlayerType.EXPLORER;

	void Update() {
		if (isServer && HauntManager.gameState == HauntManager.HauntState.HAUNT) {
			bool allHeroesDead = true;
			foreach (GamePlayer player in NetworkGame.GetPlayers()) {
				if (player.playerState == GamePlayer.PlayerType.HERO && player.gameObject.GetComponent<Damageable> ().IsDead () == false) {
					allHeroesDead = false;
				}
			}

			if (allHeroesDead) {
				//Traitor wins
				HauntManager.EndHaunt();
				winner = GamePlayer.PlayerType.TRAITOR;
			}

			if (theRing == null) {
				HauntManager.EndHaunt ();
				winner = GamePlayer.PlayerType.HERO;
			}
		}
	}

	/// <summary>
	/// Determines whether this instance can start haunt. This is determined by the haunt checking
	/// the house for what has happened. This method is called quite often, 10 times a second, for 
	/// every haunt so it should not be too intensive of a process.
	/// </summary>
	/// <returns><c>true</c> if this instance can start haunt; otherwise, <c>false</c>.</returns>
	public override bool CanStartHaunt() {
		foreach (GamePlayer player in NetworkGame.GetPlayers()) {
			foreach (GameObject item in player.GetComponent<Inventory>().items) {
				if (item != null && item.GetComponent<Item>().itemName == "Ring") {
					theRing = item;
					chosenOne = player;
					return true;
				}
			}
		}
		return false;
	}
	/// <summary>
	/// Starts the haunt. This is used in place of Start() which is used by the abstract Haunt to 
	/// setup the default state of the game for the given kind of haunt.
	/// </summary>
	public override void HauntStarted() {
		//make traitor invisible
		RpcMakeInvisible (chosenOne.gameObject);
	}

	[ClientRpc]
	public void RpcMakeInvisible(GameObject gamePlayer) {
		Debug.Log (gamePlayer);
		GameObject obj = gamePlayer.GetComponent<GamePlayer> ().playerModel;
		foreach (Renderer ren in obj.GetComponentsInChildren<Renderer>()) {
			ren.enabled = false;
		}
		foreach (SkinnedMeshRenderer ren in obj.GetComponentsInChildren<SkinnedMeshRenderer>()) {
			ren.enabled = false;
		}
		foreach (MeshRenderer ren in obj.GetComponentsInChildren<MeshRenderer>()) {
			ren.enabled = false;
		}
	}

	/// <summary>
	/// From the given list of all players in the game, select which one should become the tratior.
	/// </summary>
	/// <returns>The tratior.</returns>
	/// <param name="players">Players.</param>
	public override GamePlayer SelectTratior (GamePlayer[] players) {
		return chosenOne;
	}	


	/// <summary>
	/// Get text to display once the haunt has ended
	/// </summary>
	/// <returns>The end text.</returns>
	public override string GetEndText() {
		if (winner == GamePlayer.PlayerType.TRAITOR) {
			return "Traitor Wins! - The traitor has killed all the heroes, better luck next time";
		} else {
			return "Heroes Win! - The ring is finally destroyed, never to be seen again, goodbye and your power shall forever be gone.";
		}
	}
}
