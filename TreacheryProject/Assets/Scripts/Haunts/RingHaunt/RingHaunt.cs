using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingHaunt : Haunt {

	public static GamePlayer chosenOne = null;

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
		//Do some stuff :)
	}
	/// <summary>
	/// From the given list of all players in the game, select which one should become the tratior.
	/// </summary>
	/// <returns>The tratior.</returns>
	/// <param name="players">Players.</param>
	public override GamePlayer SelectTratior (GamePlayer[] players) {
		return chosenOne;
	}
}
