using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// A Haunt is an ending to the game that changes how the game is played. 
/// Haunts can be different types: Tratior, Cooperative, HiddenTration, FreeForAll, 
/// or a Mixed type. This is a generic haunt that has abstract methods for any 
/// created haunt to fulfuil. A created haunt must be able to check if this haunt can
/// start with given conditions of the game. A haunt must also be responsible to 
/// do setup for the game, and select a tratior if needed.
/// 
/// Haunts in the game would normally start when a player finds an omen. Since this game
/// is not turn based, it is more complex and hench each haunt is responsible to 
/// determine when they should start.
/// 
/// After a haunt starts, Players are dived into two teams, Tratior and Hero
/// </summary>
public abstract class Haunt : NetworkBehaviour {

	/// <summary>
	/// These are the different Haunt types.
	/// 
	/// Tratior: One player is trying to kill everyone.
	/// 	One player is tratior, rest are heroes
	/// 
	/// Cooperative: All playeres versus the house.
	/// 	All players are heroes
	/// 
	/// HiddenTratior: One player is trying to kill everyone but only the tratior 
	/// knows who the tratior is.
	/// 	One player is tratior, rest are heroes
	/// 
	/// FreeForAll: All players are attempting to do an objective and the last man 
	/// standing wins or the first one to complete the objective.
	/// 	All players are set to type Madman
	/// 
	/// Mixed: Any permutation of haunt not covered by the previous or is players 
	/// are switching sides during the haunt form Tratior to Heroes.
	/// 	All players are initially set to heroes
	/// 
	/// </summary>
	public enum HauntType {Tratior, Cooperative, HiddenTratior, FreeForAll, Mixed};

	/// <summary>
	/// The type, by default it is Tratior
	/// </summary>
	public HauntType type = HauntType.Tratior;
	/// <summary>
	/// The name of the haunt, by default  it is an empty string.
	/// </summary>
	public string hauntName = "";

	/// <summary>
	/// Determines whether this instance can start haunt. This is determined by the haunt checking
	/// the house for what has happened. This method is called quite often, 10 times a second, for 
	/// every haunt so it should not be too intensive of a process.
	/// </summary>
	/// <returns><c>true</c> if this instance can start haunt; otherwise, <c>false</c>.</returns>
	abstract public bool CanStartHaunt();
	/// <summary>
	/// Starts the haunt. This is used in place of Start() which is used by the abstract Haunt to 
	/// setup the default state of the game for the given kind of haunt.
	/// </summary>
	abstract public void HauntStarted();
	/// <summary>
	/// From the given list of all players in the game, select which one should become the tratior.
	/// </summary>
	/// <returns>The tratior.</returns>
	/// <param name="players">Players.</param>
	abstract public GamePlayer SelectTratior (GamePlayer[] players);
	/// <summary>
	/// Get text to display once the haunt has ended
	/// </summary>
	/// <returns>The end text.</returns>
	abstract public string GetEndText();

	// Use this for initialization
	void Start () {
		if (isServer) {
			//Debug.Log ("Starting haunt");
			GamePlayer[] players = NetworkGame.GetPlayers ();
			switch (type) {
			case HauntType.Tratior:
				for (int i = 0; i < players.Length; i++) {
					players [i].RpcSetPlayerState (GamePlayer.PlayerType.HERO);
				}
				SelectTratior (players).RpcSetPlayerState(GamePlayer.PlayerType.TRAITOR);
				break;
			case HauntType.Cooperative:
				for (int i = 0; i < players.Length; i++) {
					players [i].RpcSetPlayerState (GamePlayer.PlayerType.HERO);
				}
				break;
			case HauntType.HiddenTratior:
				for (int i = 0; i < players.Length; i++) {
					players [i].RpcSetPlayerState (GamePlayer.PlayerType.HERO);
				}
				SelectTratior (players).RpcSetPlayerState (GamePlayer.PlayerType.TRAITOR);
				break;
			case HauntType.FreeForAll:
				for (int i = 0; i < players.Length; i++) {
					players [i].RpcSetPlayerState (GamePlayer.PlayerType.MADMAN);
				}			
				break;
			case HauntType.Mixed:
				for (int i = 0; i < players.Length; i++) {
					players [i].RpcSetPlayerState (GamePlayer.PlayerType.HERO);
				}
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
