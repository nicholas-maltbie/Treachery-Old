using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Manages enabling and disabling camera and audio listener based on player upon object load.
/// </summary>
public class NetworkCharacter : NetworkBehaviour {
	public Camera playerCamera;
	public AudioListener playerAudioListener;

	// Use this for initialization
	void Start () {
		playerCamera.enabled = isLocalPlayer;
		playerAudioListener.enabled = isLocalPlayer;
	}
}
