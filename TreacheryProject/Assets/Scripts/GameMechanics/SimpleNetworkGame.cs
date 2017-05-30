using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Test class to have a simple network game.
/// </summary>
public class SimpleNetworkGame : NetworkManager {
	/// <summary>
	/// The main camera.
	/// </summary>
	public GameObject mainCamera;
	/// <summary>
	/// Is the client connected.
	/// </summary>
	private bool isConnected;

	public override void OnStartServer()
	{
		base.OnStartServer();
		Debug.Log ("Starting Server");
	}
	
	void OnConnectToServer() {
		Debug.Log("Connected to server");
		mainCamera.GetComponent<Camera>().enabled = false;
		mainCamera.GetComponent<AudioListener>().enabled = false;
	}

	void OnDisconnectedFromServer() {
		Debug.Log("Disconnected from server");
		mainCamera.GetComponent<Camera>().enabled = true;
		mainCamera.GetComponent<AudioListener>().enabled = true;
	}

	// Update is called once per frame
	void Update () {
		if (IsClientConnected () && ! isConnected) {
			isConnected = true;
			OnConnectToServer ();
		} else if (!IsClientConnected () && isConnected) {
			isConnected = false;
			OnDisconnectedFromServer();
		}
	}
}
