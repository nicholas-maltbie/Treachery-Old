/**
 * Copyright (C) 2017 Nicholas Maltbie
 * GNU GPL V3
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class handles rotating the character camera and moving the character's body when the 
 * character rotates his or her head enough.
 */
public class IKHeadMove : MonoBehaviour {

	//Should the neck turn when the head is moving?
	public bool ikActive = true;
	//Can move head
	public bool canMoveHead = true;
	//Can move body
	public bool canTurnBody = true;

	//Camera transform
	public Transform cameraTransform;
	//Transform of the character
	public Transform bodyTransform;
	//Head transform
	public Transform headBone;

	//look object position (used to point camera, just a sphere in front of the character
	private Vector3 lookPos = Vector3.forward;
	//Distance look object is placed in front of the camera, this is just some arbitrary value
	private float lookDist = 1;
	//Angle of the object in front of the character (in radians)
	private float lookAngleVert = 0;	//Angle with respect to vertical axis (left, right)
	private float lookAngleHoriz = 0;	//Angle with respect to horizontal axis (up, down)
	//Define bounds for head movement
	private float minAngleHoriz = -45, maxAngleHoriz = 35;

	//Animator for character
	private Animator animator;

	//Initialization values
	void Start()
	{
		//Get animator component
		this.animator = GetComponent<Animator> ();
		Cursor.lockState = CursorLockMode.Locked;
	}

	//a callback for calculating IK
	//This will update the character's neck to look at the ball.
	void OnAnimatorIK()
	{
		if(animator) {
			//if the IK is active, set the position and rotation directly to the goal. 
			if(ikActive) {
				//Set look weight (speed)
				animator.SetLookAtWeight(1);
				//Set look target at the look object
				animator.SetLookAtPosition(lookPos);  
			}
		}
	}

	//On update, move the look object  based on the mouse movement
	void Update()
	{
		float changeVert = 0;
		if (canMoveHead) {
			changeVert = Input.GetAxis ("Mouse X");
			//Update camera angle based on mouse movement
			lookAngleVert += changeVert;
			lookAngleHoriz -= Input.GetAxis ("Mouse Y");

			//bound values
			float bodyVert = bodyTransform.eulerAngles.y, bodyHoriz = 
				bodyTransform.eulerAngles.x;
			
			lookAngleHoriz = Mathf.Max (minAngleHoriz, Mathf.Min (maxAngleHoriz,
				lookAngleHoriz - bodyHoriz)) + bodyHoriz;

			//Update position of the look transform based on new look angles
			lookPos = cameraTransform.position + Quaternion.Euler(lookAngleHoriz,
				lookAngleVert, 0) * Vector3.forward * lookDist;
		}

		if (canTurnBody) {
			//Rotate body towards camera angle at a speed of bodyRotateSpeed
			//assume default no rotation
			float dir = 0;
			//If player is moving left, negative roation
			if (bodyTransform.eulerAngles.y - lookAngleVert > 0)
				dir = -1;
			//positive if moving right
			else if (bodyTransform.eulerAngles.y - lookAngleVert < 0)
				dir = 1;
			//Rotate body transform by at most body rotation speed to close gap between 
			// direction of the camera and direction of the character
			bodyTransform.eulerAngles = new Vector3(bodyTransform.eulerAngles.x, 
				lookAngleVert, bodyTransform.eulerAngles.z);

			//Update animator informaiton to allow rotation animation.
			if (Mathf.Abs (bodyTransform.eulerAngles.y - lookAngleVert) > 0)
				animator.SetFloat ("rotate", changeVert);
			else
				animator.SetFloat ("rotate", 0);
		}
		//Update camera angle
		cameraTransform.eulerAngles = new Vector3 (lookAngleHoriz - bodyTransform.eulerAngles.x, cameraTransform.eulerAngles.y, 0);
		cameraTransform.position = headBone.position;
		

	}
}
