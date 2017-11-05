using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

/// <summary>
/// Game options for running the game.
/// </summary>
public class GameOptions : MonoBehaviour {
	/// <summary>
	/// Game state, online if connected offline if not.
	/// </summary>
	public enum GameState {OFFLINE, ONLINE};
	/// <summary>
	/// The current state of the game.
	/// </summary>
	private GameState currentState = GameState.OFFLINE;
	/// <summary>
	/// Styles for different Graphical boxes.
	/// </summary>
	private GUIStyle style = new GUIStyle(), squareStyle = new GUIStyle();
	/// <summary>
	/// The camera animator to move the camera on animation.
	/// </summary>
	public Animator cameraAnimator;
	/// <summary>
	/// Managing the menu states.
	/// </summary>
	private bool wasConnected;
	/// <summary>
	/// The network manager for managing connections to the network.
	/// </summary>
	private NetworkManager networkManager;
	/// <summary>
	/// The game camera.
	/// </summary>
	public Camera camera;
	/// <summary>
	/// The listener of the player.
	/// </summary>
	public AudioListener listener;

	public GameObject creditsScreen, titleScreen;

	NetworkClient client;
	NetworkClient server;

	public Text serverIp, serverPort, joinIp, joinPort, joinError, hostError;

	public void HostGame() {
		if (client == null) {
			int port = 7777;
			if (serverIp.text.Length == 0) {
				networkManager.serverBindAddress = serverIp.text;
				networkManager.serverBindToIP = true;
			} else {
				networkManager.serverBindToIP = false;
			}

			if (joinPort.text.Length == 0 || Int32.TryParse (joinPort.text, out port)) {
				networkManager.networkPort = port;
				hostError.text = "Attempting to host server";
				server = networkManager.StartHost ();
				server.RegisterHandler (MsgType.Error, OnHostError);
			} else {
				hostError.text = "Port " + joinPort.text + " is not a valid number";
			}
		}
		else {
			hostError.text = "Alerady attempting to connect...";
		}
	}

	public void DisableStartScreen() {
		creditsScreen.SetActive (false);
		titleScreen.SetActive (false);
	}

	public void OnHostError(NetworkMessage error) {
		ErrorMessage msg = error.ReadMessage<ErrorMessage> ();
		hostError.text = "Could not host server: " + msg.errorCode +
			"\n" + ((NetworkError)msg.errorCode).ToString();
		server = null;

	}

	public void JoinGame() {
		if (server == null) {
			if (client == null) {
				networkManager.networkAddress = joinIp.text;
				int port = 7777;
				if (joinIp.text.Length == 0) {
					joinError.text = "Must enter a valid ip";
					return;
				}
				if (joinPort.text.Length == 0 || Int32.TryParse (joinPort.text, out port)) {
					networkManager.networkPort = port;
					joinError.text = "Attempting to join...";
					client = networkManager.StartClient ();
					client.RegisterHandler (MsgType.Error, OnJoinError);
					client.RegisterHandler (MsgType.Disconnect, OnJoinDisconnect);
				} else {
					joinError.text = "Port " + joinPort.text + " is not a valid number";
				}
			} else {
				joinError.text = "Alerady attempting to connect...";
			}
		}
		else {
			joinError.text = "Already trying to host a server";
		}
			
	}

	public void OnJoinDisconnect(NetworkMessage message) {
		joinError.text = "Could not connect to server: " +
			"\nError due to timeout";
		client = null;
	}

	public void OnHost(NetworkConnection conn, NetworkReader reader)
	{
		DisableStartScreen ();
		Debug.Log("Hosting server");
	}

	public void OnJoinError(NetworkMessage error)
	{
		ErrorMessage msg = error.ReadMessage<ErrorMessage> ();
		joinError.text = "Could not connect to server: " + msg.errorCode +
			"\n" + ((NetworkError)msg.errorCode).ToString();
		client = null;
	}


	public void Exit() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_WEBPLAYER
		Application.OpenURL(webplayerQuitURL);
		#else
		Application.Quit();
		#endif 
	}

	public void Disconnect() {
		Application.LoadLevel(networkManager.offlineScene);
		titleScreen.SetActive (true);
		currentState = GameState.OFFLINE;
		camera.enabled = true;
		listener.enabled = true;
		networkManager.StopHost();
		networkManager.StopClient();
		networkManager.StopServer();
	}

	public void CanDisplay() {
		if (cameraAnimator.GetInteger ("State") == 1) {
			creditsScreen.SetActive (true);
		} else if (cameraAnimator.GetInteger ("State") == 0) {
			titleScreen.SetActive (true);
		}
	}
	
	public void OpenCredits() {
		cameraAnimator.SetInteger ("State", 1);
		titleScreen.SetActive (false);
	}
	
	public void CloseCredits() {
		cameraAnimator.SetInteger ("State", 0);
		creditsScreen.SetActive (false);
	}
	
	void Start () {
		networkManager = GetComponent<NetworkGame> ();
		camera = GetComponent<Camera> ();
		listener = GetComponent<AudioListener> ();
	}
	
	void Update () {
		if (networkManager.isNetworkActive != wasConnected) {
			if(wasConnected) {
				currentState = GameState.OFFLINE;
			}
			else {
				currentState = GameState.ONLINE;
			}

			wasConnected = networkManager.isNetworkActive;
		}

		switch (currentState) {
		case GameState.ONLINE:
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				//ToggleMenu();
			}
			break;
		}
	}
}
