using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public int selected;
	public Item[] items;
	public GameObject held;
	public Transform hand, backpack;

	public void DropItem(GameObject item)
	{
		item.GetComponent<Item>().DropItem (gameObject);
		item.transform.position = hand.transform.position;
		item.transform.parent = null;
		for (int i = 0; i < items.Length; i++) {
			if(items[i] == item.GetComponent<Item>())
			{
				items[i] = null;
				if (i == selected) {
					held = null;
					HoldItem(null);
				}
			}
		}
	}

	public void HoldItem(GameObject item)
	{
		if (held != null) {
			held.transform.position = backpack.transform.position;
			held.transform.eulerAngles = gameObject.transform.eulerAngles;
			held.SendMessage("PutInBag");
		}
		if (item != null) {
			held = item;
			held.transform.position = hand.transform.position;
			held.transform.eulerAngles = hand.transform.eulerAngles;
			held.SendMessage ("HoldItem");
		}
		held = item;
	}

	public void PickupItem(GameObject item)
	{
		PickupItem (item, -1);
	}

	public void PickupItem(GameObject item, int index)
	{
		if (index == -1)
			index = AddItem (item.GetComponent<Item> ());
		else
			items [index] = item.GetComponent<Item>();
		if (index == selected) {
			HoldItem(item);
		}
		item.transform.position = backpack.transform.position;
		item.transform.eulerAngles = gameObject.transform.eulerAngles;
		item.transform.parent = gameObject.transform;
	}

	public int AddItem(Item item)
	{
		for(int i = items.Length - 1; i >= 0; i--)
		{
			if(items[i] == null)
			{
				items[i] = item;
				return i;
			}
		}
		return -1;
	}

	public bool HasOpenSpace()
	{
		foreach (Item i in items) {
			if(i == null)
				return true;
		}
		return false;
	}

	public bool HasItem(Item item)
	{
		for (int i = 0; i < items.Length; i++) {
			if(items[i] == item)
				return true;
		}
		return false;
	}
}
