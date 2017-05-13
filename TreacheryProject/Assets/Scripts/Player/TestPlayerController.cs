using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour {

	public GameObject camera;
	private float walkSpeed = 4.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//basic movement vector
		Vector3 movement = Vector3.zero;

		if (Input.GetAxis("Vertical") == 1) {
			//move forward
		}
		else if (Input.GetAxis ("Vertical") == -1) {
			//move backward
		}

		//Move the gameobject movement in the direction of camera times
		//  walkspeed times deltaTime.
		
	}
}
