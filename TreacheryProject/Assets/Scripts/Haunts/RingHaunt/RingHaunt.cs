using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class RingHaunt : Haunt {

	public static GamePlayer chosenOne = null;
	public static GameObject theRing = null;

	private float respawnTime = 10f;
	private static Dictionary<GamePlayer, Vector3> traitorSpawns = new Dictionary<GamePlayer, Vector3> ();
	private static Dictionary<GamePlayer, float> traitorDeadTimes = new Dictionary<GamePlayer, float>();
	[SyncVar]
	public GamePlayer.PlayerType winner = GamePlayer.PlayerType.EXPLORER;

	void Update() {
		if (isServer && HauntManager.gameState == HauntManager.HauntState.HAUNT) {
			//Check if traitor(s) have won
			bool allHeroesDead = true;
			foreach (GamePlayer player in NetworkGame.GetPlayers()) {
				if (player.playerState == GamePlayer.PlayerType.HERO && player.gameObject.GetComponent<Damageable> ().IsDead () == false) {
					allHeroesDead = false;
				}
			}
			/*if (allHeroesDead) {
				//Traitor wins
				HauntManager.EndHaunt();
				winner = GamePlayer.PlayerType.TRAITOR;
			}*/

			//Check if heroes have won
			if (theRing == null) {
				HauntManager.EndHaunt ();
				winner = GamePlayer.PlayerType.HERO;
			}

			//Update ring and invisibility
			foreach (GamePlayer player in NetworkGame.GetPlayers()) {
				if (player.playerState == GamePlayer.PlayerType.TRAITOR) {
					bool hasRing = false;
					foreach (GameObject item in player.GetComponent<Inventory>().items) {
						if (item != null) {
							if (item.GetComponent<Item> ().itemName == "Ring") {
								hasRing = true;
								item.GetComponent<Item> ().canDrop = false;
							}
						}
					}
					if (hasRing) {
						RpcMakeInvisible (chosenOne.gameObject);
					} else {
						RpcMakeVisible (chosenOne.gameObject);
					}
				}
				if (player.playerState == GamePlayer.PlayerType.HERO) {
					foreach (GameObject item in player.GetComponent<Inventory>().items) {
						if (item != null && item.GetComponent<Item> ().itemName == "Ring") {
							item.GetComponent<Item> ().canDrop = true;
						}
					}
				}
			}

			//Update traitor death times
			foreach(GamePlayer player in NetworkGame.GetPlayers()) {
				if (player.playerState == GamePlayer.PlayerType.TRAITOR && 
					player.GetComponent<Damageable>().IsDead()) {

					if (traitorDeadTimes [player] == 0) {
						//If the traitor just died
						player.AddTimedMessage("You are dead, but it's only temporary", respawnTime);
					}
					traitorDeadTimes [player] = traitorDeadTimes [player] + Time.deltaTime;
					if (traitorDeadTimes [player] >= respawnTime) {
						traitorDeadTimes [player] = 0;
						player.ServerRespawn ();
						player.gameObject.transform.position = traitorSpawns [player];
						player.AddTimedMessage ("You are back in the world of the living, for now...", 2.5f);
					}
				}
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
					traitorSpawns[player] = player.gameObject.transform.position;
					traitorDeadTimes [player] = 0;
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
	}

	[ClientRpc]
	public void RpcMakeInvisible(GameObject gamePlayer) {
		GameObject obj = gamePlayer.GetComponent<GamePlayer> ().playerModel;
		obj.GetComponent<Invisible> ().SetState (true);
	}

	[ClientRpc]
	public void RpcMakeVisible(GameObject gamePlayer) {
		GameObject obj = gamePlayer.GetComponent<GamePlayer> ().playerModel;
		obj.GetComponent<Invisible> ().SetState (false);
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
