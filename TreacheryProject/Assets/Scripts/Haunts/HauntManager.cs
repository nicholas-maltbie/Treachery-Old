using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// There is only one HauntManager in a game and it is responsible to determine 
/// when a haunt should start and which haunt should start. This is a 
/// server isde process and 
/// </summary>
public class HauntManager : NetworkBehaviour {

	public enum HauntState {EXPLORE, PREP, HAUNT, END};

	/// <summary>
	/// All possible haunts
	/// </summary>
	public Haunt[] haunts;
	/// <summary>
	/// the delay (in seconds) between each check for if the haunts should start.
	/// </summary>
	public float checkInterval = .1f;
	/// <summary>
	/// Has the haunt started.
	/// </summary>
	public static HauntState gameState = HauntState.EXPLORE;
	public static HauntState prevState = HauntState.EXPLORE;
	[SyncVar]
	private HauntState currState;
	/// <summary>
	/// The haunt.
	/// </summary>
	public static GameObject gameHaunt;

	public static HashSet<GamePlayer> ready = new HashSet<GamePlayer> ();

	// Use this for initialization
	void Start () {
	
	}

	[ServerCallback]
	public static void EndHaunt() {
		gameState = HauntState.END;
	}

	[ServerCallback]
	public static void PlayerReady(GamePlayer player) {
		if (gameState == HauntState.PREP) 
			ready.Add (player);
	}

	[ClientRpc]
	public void RpcHauntEnded() {
		foreach (GamePlayer player in NetworkGame.GetPlayers()) {
			if (player.isLocalPlayer) {
				player.DisplayHauntInfo (gameHaunt.GetComponent<Haunt>());
			}
		}
	}

	/// <summary>
	/// Called on clients when the haunt starts.
	/// </summary>
	[ClientRpc]
	public void RpcHauntStarted(GameObject haunt)
	{
		gameHaunt = haunt;
		Time.timeScale = 0;
		foreach (GamePlayer player in NetworkGame.GetPlayers()) {
			if (player.isLocalPlayer) {
				player.DisplayHauntInfo (gameHaunt.GetComponent<Haunt>());
			}
		}
	}

	[ClientRpc]
	public void RpcPrepEnd()
	{
		Time.timeScale = 1;
		foreach (GamePlayer player in NetworkGame.GetPlayers()) {
			if (player.isLocalPlayer) {
				player.StopHauntPrep ();
			}
		}
	}

	private float elapsed;

	// Update is called once per frame
	void Update () {
		if (!isServer) {
			gameState = currState;
		} else {
			currState = gameState;
		}
		///Check if the haunt has started yet.
		if (isServer && gameState == HauntState.EXPLORE) {
			elapsed += Time.deltaTime;
			if (elapsed >= checkInterval) {
				//Debug.Log ("Checking Haunts");
				foreach (Haunt haunt in haunts) {
					if (haunt.CanStartHaunt ()) {
						//Debug.Log ("Started Haunt");
						gameHaunt = GameObject.Instantiate (haunt.gameObject);
						NetworkServer.Spawn (gameHaunt);
						gameState = HauntState.PREP;

						gameHaunt.GetComponent<Haunt>().HauntStarted ();
						RpcHauntStarted (gameHaunt);
						return;
					}
				}
				elapsed = 0;
			}
		} else if (isServer && gameState == HauntState.PREP) {
			if (ready.Count == NetworkGame.GetPlayers ().Length) {
				gameState = HauntState.HAUNT;
				RpcPrepEnd ();
			}
		} else if (isServer && gameState == HauntState.END) {
			if (prevState != HauntState.END) {
				RpcHauntEnded ();
			}
		}
		prevState = gameState;
	}
}
