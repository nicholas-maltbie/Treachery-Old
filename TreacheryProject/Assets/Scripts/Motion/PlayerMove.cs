using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script manages how a player can move
 */
public class PlayerMove : MonoBehaviour {

	public float moveSpeed = 1.0f;
	public float jumpSpeed = 3.0f;
	public float minDownTimeBeforeJump = 1.0f;

	public bool canMove = true;
	public bool canJump = true;
	
	private float groundTime = 0.0f;
	private bool prevGround = true;

	public Rigidbody characterRigidbody;
	public Transform characterTransform;
	public Animator characterAnimator;
	public CapsuleCollider characterCollider;

	bool IsGrounded() {
		RaycastHit hit;
		if (Physics.SphereCast(characterTransform.position + new Vector3(0, characterCollider.height, 0), 
				characterCollider.radius, Vector3.down, out hit, Mathf.Infinity)) {
			return hit.distance <= characterCollider.height + 0.1f;
		}
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
		bool grounded = IsGrounded ();


		if (characterAnimator.GetBool ("jump") && grounded) {
			characterAnimator.SetBool ("jump", false);
		}else if (jump == 1 && canJump && grounded && groundTime >= minDownTimeBeforeJump) {
			characterAnimator.SetBool ("jump", true);
			characterRigidbody.velocity = new Vector3 (characterRigidbody.velocity.x, 
				jumpSpeed, characterRigidbody.velocity.z);
		}

		if (grounded) {
			groundTime += Time.deltaTime;
		} else {
			groundTime = 0;
		}

		//Move the player if he or she can move
		if (canMove && grounded) {
			//Add vertical movement
			move += dz * characterTransform.forward;
			//Add horizontal movement
			move += dx * characterTransform.right;

			//Set movement distance to movespeed
			move = move.normalized * moveSpeed;

			//Translate character based on move.
			characterRigidbody.velocity = new Vector3(move.x, characterRigidbody.velocity.y, move.z);
		}

		//set animator value walking to true (just for now though)
		characterAnimator.SetBool("walking", Mathf.Abs(dz) + Mathf.Abs(dx) > 0);
		//Set animator vx and vz
		characterAnimator.SetFloat ("vx", dx);
		characterAnimator.SetFloat ("vz", dz);

		//Save the previous grounded state of the character
		prevGround = IsGrounded ();
	}
}
