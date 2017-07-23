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

	/// <summary>
	/// Volume for sound
	/// </summary>
	public float baseVolume;
	/// <summary>
	/// Acceleartion due to Gravity
	/// </summary>
	public float gravity = 1.0f;
	/// <summary>
	/// Distance between a hand and player.
	/// </summary>
	public float handDist = 0.1f;
	/// <summary>
	/// Speed at which the player moves
	/// </summary>
	public float moveSpeed = 1.0f;
	/// <summary>
	/// Acceleartion of a player jump
	/// </summary>
	public float jumpSpeed = 3.0f;
	/// <summary>
	/// Minimum cooldown between jumps
	/// </summary>
	public float minDownTimeBeforeJump = 1.0f;

	/// <summary>
	/// Can the player Move.
	/// </summary>
	public bool canMove = true;
	/// <summary>
	/// Can the player Jump
	/// </summary>
	public bool canJump = true;
	/// <summary>
	/// Can the player move his/her head
	/// </summary>
	public bool canMoveHead = true;
	/// <summary>
	/// Can the player turn their body
	/// </summary>
	public bool canTurnBody = true;

	/// <summary>
	/// Current look angle of the character
	/// </summary>
	private Quaternion look = Quaternion.identity;

	/// <summary>
	/// Copy of player inventory
	/// </summary>
	public Inventory inv;

	/// <summary>
	/// Transform of the player hand pivot
	/// </summary>
	public Transform handPivotPos;
	/// <summary>
	/// Camera position
	/// </summary>
	public Transform cameraTransform;
	/// <summary>
	/// Hand position
	/// </summary>
	public Transform handTransform;
	/// <summary>
	/// Head position.
	/// </summary>
	public Transform headBone;

	/// <summary>
	/// Distance look object is placed in front of the camera, this is just some arbitrary value
	/// </summary>
	private float lookDist = 1;
	/// <summary>
	/// Angle of the object in front of the character (in radians)
	/// Angle with respect to vertical axis (left, right) and Angle with respect to horizontal axis (up, down)
	/// </summary>
	private float lookAngleVert = 0, lookAngleHoriz = 0;
	/// <summary>
	/// Defined head bound movement
	/// </summary>
	private float minAngleHoriz = -80, maxAngleHoriz = 40;
	/// <summary>
	/// Was the player grounded last frame
	/// </summary>
	private bool wasGrounded = true;
	/// <summary>
	/// How long has the character been on the ground
	/// </summary>
	private float groundTime = 0.0f;
	/// <summary>
	/// Vertical velocity of the character
	/// </summary>
	private float verticalVel = 0.0f;


	/// <summary>
	/// Object that makes foot sounds for this player.
	/// </summary>
	private FootSounds footSounds;
	/// <summary>
	/// Character's movement controller
	/// </summary>
	private CharacterController characterController;
	/// <summary>
	/// The head move script.
	/// </summary>
	public IKHeadMove headMoveScript;
	/// <summary>
	/// The hand move script.
	/// </summary>
	public IKHandMove handMoveScript;

	/// <summary>
	/// Character animator, for animating the character.
	/// The animator must have the following parameters.
	/// vx - x velocity
	/// vz - z velocity
	/// walking - Is the character walking
	/// jump - Is the character currently jumping/airborne
	/// </summary>
	public Animator characterAnimator;

	/// <summary>
	/// Character base that moves
	/// </summary>
	public Transform characterTransform;

	void Start() {
		characterController = GetComponent<CharacterController> ();
		footSounds = GetComponent<FootSounds> ();
		baseVolume = footSounds.volumeMult;
	}

	/// <summary>
	/// Function to check if the character is currently grounded
	/// </summary>
	/// <returns><c>true</c> if this player is grounded; otherwise, <c>false</c>.</returns>
	bool IsGrounded() {
		return characterController.isGrounded;
	}

	[Command]
	public void CmdSetLook(Quaternion look) {
		RpcSetLook (look);
	}

	[ClientRpc]
	public void RpcSetLook(Quaternion look) {
		this.look = look;
	}

	[Command]
	public void CmdSetHandIK(bool active) {
		RpcSetHandIK (active);
	}

	[ClientRpc]
	public void RpcSetHandIK(bool active) {
		if (!isLocalPlayer) {
			handMoveScript.ikActive = active;
		}
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

				CmdSetLook (Quaternion.Euler (lookAngleHoriz, lookAngleVert, 0));
				CmdSetHandIK (inv.IsHoldingItem () || GetComponent<PlayerAttack>().meleeAttack);
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

			GetComponentInChildren<FootSounds> ().volumeMult = baseVolume * speed * speed;

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
			if (canMove) {
				characterController.Move (new Vector3 (move.x, verticalVel, move.z) * Time.deltaTime);
			}

			//set animator value walking to true (just for now though)
			characterAnimator.SetBool ("walking", Mathf.Abs (dz) + Mathf.Abs (dx) > 0);
			//Set animator vx and vz
			characterAnimator.SetFloat ("vx", dx);
			characterAnimator.SetFloat ("vz", dz + sprint);

			wasGrounded = grounded;
		}
		Vector3 lookPos = cameraTransform.position + look * Vector3.forward * lookDist;
		Vector3 handPos = handPivotPos.position + look * Vector3.forward * handDist;
		handTransform.position = handPos;
		handTransform.rotation = look;
		handMoveScript.ikActive = inv.IsHoldingItem () || GetComponent<PlayerAttack>().meleeAttack;
		headMoveScript.lookPos = lookPos;
	}
}
