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
	//look object position (used to point camera, just a sphere in front of the character
	public Vector3 lookPos = Vector3.forward;
	//Transform of the character
	public Transform bodyTransform;

	//Distance look object is placed in front of the camera, this is just some arbitrary value
	private float lookDist = 1;
	//Angle of the object in front of the character (in radians)
	private float lookAngleVert = 0;	//Angle with respect to vertical axis (left, right)
	private float lookAngleHoriz = 0;	//Angle with respect to horizontal axis (up, down)
	//Define bounds for head movement
	private float minAngleVert = -45, maxAngleVert = 45, minAngleHoriz = -45, maxAngleHoriz = 35;
	//Define body rotate speed (In degrees per second)
	public float bodyRotateSpeed = 90f;

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
		if (canMoveHead) {
			//Update camera angle based on mouse movement
			lookAngleVert += Input.GetAxis ("Mouse X");
			lookAngleHoriz -= Input.GetAxis ("Mouse Y");

			//bound values
			float bodyVert = bodyTransform.eulerAngles.y, bodyHoriz = bodyTransform.eulerAngles.z;

			lookAngleVert = Mathf.Max (minAngleVert, Mathf.Min(maxAngleVert, lookAngleVert - bodyVert)) + bodyVert;
			lookAngleHoriz = Mathf.Max (minAngleHoriz, Mathf.Min (maxAngleHoriz, lookAngleHoriz - bodyHoriz)) + bodyHoriz;

			//Update position of the look transform based on new look angles
			lookPos = cameraTransform.position + Quaternion.Euler(lookAngleHoriz, lookAngleVert, 0) * Vector3.forward * lookDist;
		}

		if (canTurnBody) {
			//Rotate body towards camera angle at a speed of bodyRotateSpeed
			float dir = 0;
			if (bodyTransform.eulerAngles.y - lookAngleVert > 0)
				dir = -1;
			else if (bodyTransform.eulerAngles.y - lookAngleVert < 0)
				dir = 1;
			bodyTransform.Rotate(0, dir * Mathf.Min(Mathf.Abs(bodyTransform.eulerAngles.y - lookAngleVert), 
				bodyRotateSpeed * Time.deltaTime), 0);
			if (Mathf.Abs (bodyTransform.eulerAngles.y - lookAngleVert) > 0.1f)
				animator.SetFloat ("rotate", 
					Mathf.Min(dir * (Mathf.Abs(animator.GetFloat("rotate")) + Time.deltaTime), 1));
			else
				animator.SetFloat ("rotate", 
					Mathf.Max(dir * (Mathf.Abs(animator.GetFloat("rotate")) - Time.deltaTime), 0));
		}
		

	}
}
