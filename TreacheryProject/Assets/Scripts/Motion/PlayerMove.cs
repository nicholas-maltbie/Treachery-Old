using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * This script manages how a player can move
 */
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(FootSounds))]
public class PlayerMove : NetworkBehaviour {

	/**
	 * Gravity on the character (Acceleration due to gravity)
	 */
	public float gravity = 1.0f;
	private float handDist = 0.25f;
	/**
	 * Speed that the character moves
	 */
	public float moveSpeed = 1.0f;
	public Inventory inv;
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
	public Transform handPivotPos;
	//Can move head
	public bool canMoveHead = true;
	//Can move body
	public bool canTurnBody = true;

	//Camera transform
	public Transform cameraTransform;
	//Hand transform
	public Transform handTransform;
	//Head transform
	public Transform headBone;
	
	//Distance look object is placed in front of the camera, this is just some arbitrary value
	private float lookDist = 1;
	//Angle of the object in front of the character (in radians)
	private float lookAngleVert = 0;	//Angle with respect to vertical axis (left, right)
	private float lookAngleHoriz = 0;	//Angle with respect to horizontal axis (up, down)
	//Define bounds for head movement
	private float minAngleHoriz = -80, maxAngleHoriz = 40;


	/// <summary>
	/// Was the player grounded last frame
	/// </summary>
	private bool wasGrounded = true;
	/**
	 * How long has the character been on the ground
	 */
	private float groundTime = 0.0f;
	/**
	 * Vertical velocity of the character
	 */
	private float verticalVel = 0.0f;


	/// <summary>
	/// Object that makes foot sounds for this player.
	/// </summary>
	private FootSounds footSounds;
	/**
	 * Character's movement controller
	 */
	private CharacterController characterController;
	
	public IKHeadMove headMoveScript;
	public IKHandMove handMoveScript;
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
	 * Character base that moves
	 */
	public Transform characterTransform;

	void Start() {
		characterController = GetComponent<CharacterController> ();
		footSounds = GetComponent<FootSounds> ();
	}

	/**
	 * Function to check if the character is currently grounded
	 */
	bool IsGrounded() {
		return characterController.isGrounded;
	}

	[Command]
	public void CmdSetHandIK(Vector3 handPos, Quaternion handRot, bool active) {
		RpcSetHandIK (handPos, handRot, active);
	}

	[ClientRpc]
	public void RpcSetHandIK(Vector3 handPos, Quaternion handRot, bool active) {
		if (!isLocalPlayer) {
			handTransform.position = handPos;
			handTransform.rotation = handRot;
			handMoveScript.active = active;
		}
	}

	[Command]
	public void CmdSetHeadIK(Vector3 lookPos) {
		RpcSetHeadIK (lookPos);
	}

	[ClientRpc]
	public void RpcSetHeadIK(Vector3 lookPos) {
		headMoveScript.lookPos = lookPos;
	}

	// Update is called once per frame
	void Update () {
		//Only move if local player
		if (isLocalPlayer) {

			float changeVert = 0;
			if (canMoveHead) {
				changeVert = Input.GetAxis ("Mouse X");
				//Update camera angle based on mouse movement
				lookAngleVert += changeVert;
				lookAngleHoriz -= Input.GetAxis ("Mouse Y");

				//bound values
				float bodyHoriz = characterTransform.eulerAngles.x;

				lookAngleHoriz = Mathf.Max (minAngleHoriz, Mathf.Min (maxAngleHoriz,
					lookAngleHoriz - bodyHoriz)) + bodyHoriz;

				//Update position of the look transform based on new look angles
				Vector3 lookPos = cameraTransform.position + Quaternion.Euler (lookAngleHoriz,
					lookAngleVert, 0) * Vector3.forward * lookDist;

				CmdSetHeadIK(lookPos);
				Vector3 handPos = handPivotPos.position + Quaternion.Euler(lookAngleHoriz,
					lookAngleVert, 0) * Vector3.forward * handDist;
				CmdSetHandIK (handPos, Quaternion.Euler(lookAngleHoriz, lookAngleVert, 0), inv.IsHoldingItem ());
				handTransform.position = handPos;
				handTransform.rotation = Quaternion.Euler(lookAngleHoriz, lookAngleVert, 0);
				handMoveScript.active = inv.IsHoldingItem ();
			}


			if (canTurnBody) {
				//Rotate body towards camera angle at a speed of bodyRotateSpeed
				//Rotate body transform by at most body rotation speed to close gap between 
				// direction of the camera and direction of the character
				characterTransform.eulerAngles = new Vector3 (characterTransform.eulerAngles.x, 
					lookAngleVert, characterTransform.eulerAngles.z);

				//Update animator informaiton to allow rotation animation.
				if (Mathf.Abs (characterTransform.eulerAngles.y - lookAngleVert) > 0)
					characterAnimator.SetFloat ("rotate", changeVert);
				else
					characterAnimator.SetFloat ("rotate", 0);
			}
			//Update camera angle
			cameraTransform.eulerAngles = new Vector3 (lookAngleHoriz - characterTransform.eulerAngles.x, cameraTransform.eulerAngles.y, 0);
			cameraTransform.position = headBone.position;

			//Get vertical and horizontal movement.
			float dz = Input.GetAxis ("Vertical");
			float dx = Input.GetAxis ("Horizontal");
			//does the player want to jump
			float jump = Input.GetAxis ("Jump");
			//does the player want to crouch
			float crouch = Input.GetAxis ("Crouch");
			float sprint = Input.GetAxis ("Sprint");

			float speed = moveSpeed;
			if (crouch == 1)
				speed = 0.5f * speed;
			else if (sprint == 1)
				speed = 2 * speed;

			GetComponentInChildren<FootSounds> ().volumeMult = speed * speed;

			if (crouch == 1) {
				characterAnimator.SetBool ("crouch", true);
			} else {
				characterAnimator.SetBool ("crouch", false);
			}


			//check if the player is grounded
			bool grounded = IsGrounded ();

			if (wasGrounded && !grounded) {
				footSounds.Lift ();
			} else if (!wasGrounded && grounded) {
				footSounds.Land ();
			}

			//If the player was jumping due to being airbone and is now on the ground
			// tell the animator that the character is on the ground
			if (characterAnimator.GetBool ("jump") && grounded) {
				characterAnimator.SetBool ("jump", false);
			}
			//If the player wants to jump, is permitted to jump,
			// is on the ground and has waited for the cooldown to to wear off, jump
			else if (jump == 1 && canJump && grounded && groundTime >= minDownTimeBeforeJump) {
				characterAnimator.SetBool ("jump", true);
				verticalVel = jumpSpeed;
			}

			//If the player is grounded, update ground time.
			if (grounded) {
				groundTime += Time.deltaTime;
			} else {
				//If not grounded, set ground time to zero.
				groundTime = 0;
				//have gravity act on the character
				verticalVel += -gravity * Time.deltaTime;
			}
		
			//Init move vector to zero
			Vector3 move = Vector3.zero;
			//Move the player if he or she can move
			if (canMove) {
				//Add vertical movement
				move += dz * characterTransform.forward;
				//Add horizontal movement
				move += dx * characterTransform.right;

				//Set movement distance to movespeed
				move = move.normalized * speed;
			}

			//Translate character based on move.
			characterController.Move (new Vector3 (move.x, verticalVel, move.z) * Time.deltaTime);

			//set animator value walking to true (just for now though)
			characterAnimator.SetBool ("walking", Mathf.Abs (dz) + Mathf.Abs (dx) > 0);
			//Set animator vx and vz
			characterAnimator.SetFloat ("vx", dx);
			characterAnimator.SetFloat ("vz", dz + sprint);

			wasGrounded = grounded;
		}
	}
}
