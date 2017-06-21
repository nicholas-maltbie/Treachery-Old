using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEventForward : MonoBehaviour {

	public Door parentDoor;

	void Open() {
		parentDoor.Open ();
	}

	void Close() {
		parentDoor.Close ();
	}
}
