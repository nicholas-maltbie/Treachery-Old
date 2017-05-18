using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script manages how a player can move
 */
public class PlayerMove : MonoBehaviour {

	public float moveSpeed = 1.0f;
	public float jumpSpeed = 3.0f;

	public bool canMove = true;
	public bool canJump = true;

	private float gravity = 1.0f;
	private bool prevGround = true;

	public Rigidbody characterRigidbody;
	public Transform characterTransform;
	public Animator characterAnimator;
	public Collider characterCollider;

	bool IsGrounded() {
		RaycastHit hit;

		if (Physics.Raycast (characterTransform.position, -Vector3.up, out hit))
			return hit.distance <= 0.1f;
		return false;
	}
	// Update is called once per frame
	void Update () {
		//Init move vector to zero
		Vector3 move = Vector3.zero;
		//Get vertical and horizontal movement.
		float dz = Input.GetAxis ("Vertical");
		float dx = Input.GetAxis ("Horizontal");
		float jump = Input.GetAxis ("Jump");

		if (characterAnimator.GetBool ("jump") && IsGrounded ()) {
			characterAnimator.SetBool ("jump", false);
		}else if (jump == 1 && canJump && IsGrounded ()) {
			characterAnimator.SetBool ("jump", true);
			characterRigidbody.velocity = new Vector3 (characterRigidbody.velocity.x, 
				jumpSpeed, characterRigidbody.velocity.z);
		}

		//Move the player if he or she can move
		if (canMove) {
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

		//Save the previous grounded state of the character
		prevGround = IsGrounded ();
	}
}
