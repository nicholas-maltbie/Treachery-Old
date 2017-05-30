using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour {

	public List<GameObject> items;
	private static List<GameObject> itemPrefabs;
	private static List<Item> spawnedItems = new List<Item>();

	[ServerCallback]
	public static GameObject SpawnItem()
	{
		if(HasItems())
		{
			int index = (int)(Random.value * itemPrefabs.Count);
			GameObject item = GameObject.Instantiate(itemPrefabs[index]);
			itemPrefabs.RemoveAt(index);
			spawnedItems.Add (item.GetComponent<Item>());
			NetworkServer.Spawn(item);
			return item;
		}
		return null;
	}

	[ServerCallback]
	public static bool HasItems()
	{
		return  itemPrefabs.Count > 0;
	}

	// Use this for initialization
	void Start () {
		itemPrefabs = (List<GameObject>)items.GetRange(0, items.Count);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
