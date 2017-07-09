using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// Network game that can manage itself as a client or manage a server.
/// </summary>
public class NetworkGame : NetworkManager {
	/// <summary>
	/// The current number fo players spawned.
	/// </summary>
	private int spawned;
	/// <summary>
	/// The last state of whether this client is connected.
	/// </summary>
	private bool last;

	/// <summary>
	/// The main camera.
	/// </summary>
	public GameObject mainCamera;
	/// <summary>
	/// The prefabs to register.
	/// </summary>
	public GameObject[] playerPrefabs;
	/// <summary>
	/// The item prefabs.
	/// </summary>
	public GameObject[] itemPrefabs;

	/// <summary>
	/// The players connected if this is the server.
	/// </summary>
	private static List<GameObject> players = new List<GameObject> ();

	/// <summary>
	/// Register all prefabs for the game
	/// </summary>
	void Start () {
		if (!Network.isServer) {
			for (int i = 0; i < itemPrefabs.Length; i++) {
				ClientScene.RegisterPrefab (itemPrefabs [i]);
			}
			for (int i = 0; i < playerPrefabs.Length; i++) {
				ClientScene.RegisterPrefab (playerPrefabs [i]);
			}
		}
	}

	/// <summary>
	/// Update and check to make sure if the client is connected.
	/// </summary>
	public void Update () {
		if (last != IsClientConnected()) {
			if(last)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			last = IsClientConnected();
		}
	}

	/// <summary>
	/// Gets an array of all players currently in the game by trying to
	/// identify objects tagged as "Player"
	/// </summary>
	/// <returns>The players.</returns>
	public static GameObject[] GetPlayers()
	{
		return GameObject.FindGameObjectsWithTag ("Player");
	}

	/// <summary>
	/// Raises the server add player event.
	/// When a player is added, this will create an instance of the next selected player prefab.
	/// </summary>
	/// <param name="conn">Connection of player to generate an instance for.</param>
	/// <param name="playerControllerId">Player controller identifier.</param>
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		//Debug.Log ("Spawning player");
		Transform startPos = GetStartPosition ();
		Vector3 spawnPos = new Vector3 (0, 0, 2);
		GameObject player = (GameObject)GameObject.Instantiate(playerPrefabs[spawned % playerPrefabs.Length].gameObject, spawnPos, Quaternion.identity);
		spawned++;
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		//roomGenerator.GetComponent<RoomSpawn> ().enabled = true;
	}

	public override void OnStopServer()
	{
		base.OnStopServer();
		mainCamera.GetComponent<Camera>().enabled = true;
		mainCamera.GetComponent<AudioListener>().enabled = true;
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect (conn);
		mainCamera.GetComponent<Camera>().enabled = false;
		mainCamera.GetComponent<AudioListener>().enabled = false;
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect (conn);
		mainCamera.GetComponent<Camera>().enabled = true;
		mainCamera.GetComponent<AudioListener>().enabled = true;
	}
}
