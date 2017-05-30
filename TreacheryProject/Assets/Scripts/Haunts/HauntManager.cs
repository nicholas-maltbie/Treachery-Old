using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// There is only one HauntManager in a game and it is responsible to determine 
/// when a haunt should start and which haunt should start. This is a 
/// server isde process and 
/// </summary>
public class HauntManager : NetworkBehaviour {

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
	public bool hasStarted;

	// Use this for initialization
	void Start () {
	
	}

	/// <summary>
	/// Called on clients when the haunt starts.
	/// </summary>
	[ClientRpc]
	public void RpcHauntStarted()
	{
		
	}

	private float elapsed;

	// Update is called once per frame
	void Update () {
		///Check if the haunt has started yet.
		if (isServer && !hasStarted) {
			elapsed += Time.deltaTime;
			if (elapsed >= checkInterval) {
				//Debug.Log ("Checking Haunts");
				foreach(Haunt haunt in haunts)
				{
					if (haunt.CanStartHaunt()) {
						//Debug.Log ("Started Haunt");
						GameObject gameHaunt = GameObject.Instantiate (haunt.gameObject);
						NetworkServer.Spawn (gameHaunt);
						hasStarted = true;
						RpcHauntStarted ();
						return;
					}
				}
				elapsed = 0;
			}
		}
	}
}
