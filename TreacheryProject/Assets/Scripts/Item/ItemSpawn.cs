using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ItemSpawn : NetworkBehaviour {

	public GameObject[] locations;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RoomSpawned()
	{
		if (isServer && ItemManager.HasItems()) {
			GameObject itemThing = ItemManager.SpawnItem();
			itemThing.transform.position = locations[Random.Range(0, locations.Length)].transform.position;
			NetworkServer.Spawn (itemThing);
		}
	}
}
