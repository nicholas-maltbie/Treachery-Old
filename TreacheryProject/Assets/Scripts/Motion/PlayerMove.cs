using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script manages how a player can move
 */
public class PlayerMove : MonoBehaviour {
	/**
	 * Speed that the character moves
	 */
	public float moveSpeed = 1.0f;
	/**
	 * Velocity the character takes off from the ground at
	 */
	public float jumpSpeed = 3.0f;
	/**
	 * Minimum ground time between jumps
	 */
	public float minDownTimeBeforeJump = 1.0f;

	/**
	 * Can the player currently move
	 */
	public bool canMove = true;
	/**
	 * Can the player currently jump
	 */
	public bool canJump = true;

	/**
	 * How long has the character been on the ground
	 */
	private float groundTime = 0.0f;

	/**
	 * Character's rigidbody
	 */
	public Rigidbody characterRigidbody;
	/**
	 * Character base that moves
	 */
	public Transform characterTransform;
	/**
	 * Character animator, for animating the character.
	 * The animator must have the following parameters.
	 * 	vx - x velocity
	 *  vz - z velocity
	 *  walking - Is the character walking
	 *  jump - Is the character currently jumping/airborne
	 */
	public Animator characterAnimator;
	/**
	 * Character collider
	 */
	public CapsuleCollider characterCollider;

	/**
	 * Function to check if the character is currently grounded
	 */
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
		//does the player want to jump
		float jump = Input.GetAxis ("Jump");
		//check if the player is grounded
		bool grounded = IsGrounded ();

		//If the player was jumping due to being airbone and is now on the ground
		// tell the animator that the character is on the ground
		if (characterAnimator.GetBool ("jump") && grounded) {
			characterAnimator.SetBool ("jump", false);
		}
		//If the player wants to jump, is permitted to jump,
		// is on the gorund and has waited for the cooldown to to wear off, jump
		else if (jump == 1 && canJump && grounded && groundTime >= minDownTimeBeforeJump) {
			characterAnimator.SetBool ("jump", true);
			characterRigidbody.velocity = new Vector3 (characterRigidbody.velocity.x, 
				jumpSpeed, characterRigidbody.velocity.z);
		}

		//If the player is grounded, update ground time.
		if (grounded) {
			groundTime += Time.deltaTime;
		} else {
			//If not grounded, set ground time to zero.
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
	}
}
