using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using System;

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
	/// the size of the credit boxes.
	/// </summary>
	private float creditWidth = -1, creditHeight = -1;
	/// <summary>
	/// The scroll position for viewing a scrolly box or something like that.
	/// </summary>
	private Vector2 scrollPosition = Vector2.zero;
	/// <summary>
	/// Managing the menu states.
	/// </summary>
	private bool canDisplay = true, wasConnected, isMenuOpen;
	/// <summary>
	/// The network manager for managing connections to the network.
	/// </summary>
	private NetworkManager networkManager;
	/// <summary>
	/// The credits file.
	/// </summary>
	public TextAsset creditsFile;
	/// <summary>
	/// The game camera.
	/// </summary>
	public Camera camera;
	/// <summary>
	/// The listener of the player.
	/// </summary>
	public AudioListener listener;
	/// <summary>
	/// The address for connecting to the server.
	/// </summary>
	private string address = "localhost";
	/// <summary>
	/// The credit lines.
	/// </summary>
	private string creditLines;

	/// <summary>
	/// Allows the menu to dsplay.
	/// </summary>
	public void CanDisplay()
	{
		canDisplay = true;
	}

	void OnGUI()
	{
		GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = GUI.skin.textField.fontSize = 
			GUI.skin.textArea.fontSize = 25;
		//if not connected to server
		if (currentState == GameState.OFFLINE) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			//if not in animation
			if (canDisplay) {
				//if the state is the basic menu, display buttons and allow changes
				if (cameraAnimator.GetInteger ("State") == 0) {

					camera.enabled = true;
					listener.enabled = true;

					address = GUI.TextField (new Rect (Screen.width * .21f, Screen.height * .2f, Screen.width * .19f, Screen.height * .05f), address);
					if (GUI.Button (new Rect (Screen.width * .1f, Screen.height * .1f, Screen.width * .3f, Screen.height * .05f), "Host Game")) {
						networkManager.StartHost ();
						Cursor.lockState = CursorLockMode.Locked;
						Cursor.visible = false;
					}
					else if (GUI.Button (new Rect (Screen.width * .1f, Screen.height * .2f, Screen.width * .1f, Screen.height * .05f), "Join Game")) {
						networkManager.networkAddress = address;
						networkManager.StartClient ();
						Cursor.lockState = CursorLockMode.Locked;
						Cursor.visible = false;
					}
					else if (GUI.Button (new Rect (Screen.width * .1f, Screen.height * .3f, Screen.width * .3f, Screen.height * .05f), "Credits")) {
						cameraAnimator.SetInteger ("State", 1);
						canDisplay = false;
					}
					else if (GUI.Button (new Rect (Screen.width * .1f, Screen.height * .4f, Screen.width * .3f, Screen.height * .05f), "Exit Game")) {
						#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
						#elif UNITY_WEBPLAYER
						Application.OpenURL(webplayerQuitURL);
						#else
						Application.Quit();
						#endif 
					}
				} 
				//if the display is credis, display credits and allow the user to interact
				else if (cameraAnimator.GetInteger ("State") == 1) {
						if (GUI.Button (new Rect (Screen.width * .1f, Screen.height * .1f, Screen.width * .3f, Screen.height * .05f), "Back")) {
						cameraAnimator.SetInteger ("State", 0);
						canDisplay = false;
					}
				
					if(creditHeight == -1 && creditWidth == -1)
					{
						Vector2 size = GUI.skin.textArea.CalcSize (new GUIContent (creditLines));
						creditHeight = size.y;
						creditWidth = size.x;
					}
					scrollPosition = GUI.BeginScrollView (new Rect (Screen.width * .1f, Screen.height * .2f, Math.Min (creditWidth + 30, Screen.width * .4f), 
				                                                Math.Min (creditHeight + 30, Screen.height * .7f)), scrollPosition, new Rect (0, 0, creditWidth, creditHeight));
					GUI.TextArea (new Rect (0, 0, creditWidth + 30, creditHeight + 30), creditLines);
					GUI.EndScrollView ();
			
				}
			}
		} 
		//if the user is online
		else if(currentState == GameState.ONLINE) {
			//if the menu is open, display the menu and allow the player to interact.
			if(isMenuOpen)
			{
				camera.enabled = false;
				listener.enabled = false;
				GUI.Box(new Rect(Screen.width * .4f, Screen.height * .2f, Screen.width * .2f, Screen.height * .24f), "", squareStyle);
				if(GUI.Button(new Rect(Screen.width * .42f, Screen.height * .22f, Screen.width * .16f, Screen.height * .08f), "Back To Game"))
				{
					ToggleMenu();
				}
				else if(GUI.Button(new Rect(Screen.width * .42f, Screen.height * .32f, Screen.width * .16f, Screen.height * .08f), "Disconnect"))
				{
					Application.LoadLevel(networkManager.offlineScene);
					currentState = GameState.OFFLINE;
					camera.enabled = true;
					listener.enabled = true;
					ToggleMenu();
					networkManager.StopHost();
					networkManager.StopClient();
					networkManager.StopServer();
				}

			}
			//if the menu is not open, display instructions to open the menu
			else
			{
				GUI.Label(new Rect(Screen.width * .8f, Screen.height * .05f, Screen.width * .4f, Screen.width * .1f), "Press [ESC] to open menu");
			}
		}
	}
	
	void Start () {
		style.alignment = TextAnchor.MiddleCenter;
		creditLines = creditsFile.text;
		Texture2D gray = new Texture2D (1, 1);
		gray.SetPixel (0, 0, Color.gray);
		gray.wrapMode = TextureWrapMode.Repeat;
		gray.Apply ();
		squareStyle.normal.background = gray;
		networkManager = GetComponent<NetworkManager> ();
		camera = GetComponent<Camera> ();
		listener = GetComponent<AudioListener> ();
	}
	
	/// <summary>
	/// Toggles the menu.
	/// </summary>
	public void ToggleMenu()
	{
		isMenuOpen = !isMenuOpen;
		if(isMenuOpen)
		{
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}
		else{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
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
				ToggleMenu();
			}
			break;
		}
	}
}
