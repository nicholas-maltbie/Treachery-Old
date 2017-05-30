/**
 * Copyright (C) 2017 Nicholas Maltbie
 * GNU GPL V3
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * This class handles rotating the character camera and moving the character's body when the 
 * character rotates his or her head enough.
 */
public class IKHeadMove : NetworkBehaviour {

	//Should the neck turn when the head is moving?
	public bool ikActive = true;
	//look object position (used to point camera, just a sphere in front of the character
	public Vector3 lookPos = Vector3.forward;

	//a callback for calculating IK
	//This will update the character's neck to look at the ball.
	void OnAnimatorIK()
	{
		if(GetComponent<Animator>()) {
			//if the IK is active, set the position and rotation directly to the goal. 
			if(ikActive) {
				//Set look weight (speed)
				GetComponent<Animator>().SetLookAtWeight(1);
				//Set look target at the look object
				GetComponent<Animator>().SetLookAtPosition(lookPos);  
			}
		}
	}
}
