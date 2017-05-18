using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script manages how a player can move
 */
public class PlayerMove : MonoBehaviour {

	public float moveSpeed = 1.0f;
	public bool canMove = true;

	private bool isOnGround = true;
	private float gravity = 1.0f;

	public Transform characterTransform;
	public Animator characterAnimator;

	// Update is called once per frame
	void Update () {
		//Init move vector to zero
		Vector3 move = Vector3.zero;
		//Get vertical and horizontal movement.
		float dz = Input.GetAxis ("Vertical");
		float dx = Input.GetAxis ("Horizontal");
		//Move the player if he or she can move
		if (canMove && isOnGround) {
			//Add vertical movement
			move += dz * characterTransform.forward;
			//Add horizontal movement
			move += dx * characterTransform.right;

			//Set movement distance to movespeed * time elapsed
			move = move.normalized * moveSpeed * Time.deltaTime;
		}
		//set animator value walking to true (just for now though)
		characterAnimator.SetBool("walking", Mathf.Abs(dz) + Mathf.Abs(dx) > 0);
		//Set animator vx and vz
		characterAnimator.SetFloat ("vx", dx);
		characterAnimator.SetFloat ("vz", dz);

		//Translate character based on move.
		characterTransform.position += move;
	}
}
