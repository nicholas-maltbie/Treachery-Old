using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class PlayerAttack : NetworkBehaviour {

	public GamePlayer player;
	public Damageable self;
	public IKHandMove handMover;
	public PlayerMove playerMove;

	private float baseHandDist;
	public bool meleeAttack;

	private Damageable lastHit;

	public Transform playerCamera;

	private float meleeAttackAnim = 0.35f;
	private float meleeAttackElapsed = 0f;

	private float meleeAttackReach = 0.5f;
	public float meleeAttackTime = 0.5f;
	public float meleeAttackCooldown = 1.5f;
	public float meleeAttackRange = 2f;
	public int meleeDamage = 10;

	private float currAttackTime = 0.0f;
	private float currAttackCooldown = 2.5f;
	private float attackTime = 0;

	private GamePlayer.Action attack;
	private bool isAttacking;

	public static GamePlayer.Action MakeAttack(GameObject attacker, string attackMessage) {
		GamePlayer.Action act = new GamePlayer.Action ("Attack", attacker, false);
		act.canMove = true;
		act.canCameraTurn = true;
		act.canDrop = false;
		act.canTurnBody = true;
		act.canJump = false;
		act.canInteract = false;
		act.canSwitchItems = false;
		act.canUseItems = false;
		act.canMelee = false;
		act.actionMessage = attackMessage;
		return act;
	}

	[ServerCallback]
	public void StartAttack(string attackMessage) {
		attack = PlayerAttack.MakeAttack(player.gameObject, attackMessage);
		isAttacking = true;
		player.SetAction (attack);
	}

	[ServerCallback]
	public void Interrupt(string action) {
		if (action == attack.name) {
			attackTime = 0;
			isAttacking = false;
			player.EndAction (currAttackCooldown, "Attack Interrupted");
		}
	}

	[ClientRpc]
	public void RpcMeleeAttacMotion() {
		handMover.ikActive = true;
		meleeAttack = true;
		meleeAttackElapsed = 0;
	}

	[Command]
	public void CmdAttemptMeleeAttack() {
		if (player.GetActionState() == GamePlayer.ActionState.FREE) {
			currAttackCooldown = meleeAttackCooldown;
			currAttackTime = meleeAttackTime;
			StartAttack ("Melee Attack");

			Damageable target = GetLooking (meleeAttackRange);
			RpcMeleeAttacMotion ();
			if (target != null) {
				target.DamageHealth (meleeDamage);
				lastHit = target;
			}
			else {
				lastHit = null;
			}
		}
	}

	public Damageable GetLooking(float dist) {
		RaycastHit hit;
		if (Physics.SphereCast (
			   playerCamera.position, 
			   .1f, 
				playerCamera.forward, 
				out hit,
			   dist, 
			   Physics.DefaultRaycastLayers, 
			   QueryTriggerInteraction.Collide)) {
			DamageCollider col = hit.collider.gameObject.GetComponentInChildren<DamageCollider> ();
			if (col != null && col.damageable != self && col.damageable.canBeAttacked) {
				return col.damageable;
			}
		}
		return null;
	}

	void Start() {
		baseHandDist = playerMove.handDist;
	}

	void Update() {
		if (isServer) {
			if (isAttacking) {
				attackTime += Time.deltaTime;
				if (attackTime >= currAttackTime) {
					attackTime = 0;
					isAttacking = false;
					string cooldownMessage = "Missed Target";
					if (lastHit != null) {
						cooldownMessage = "Attacked " + lastHit.name;
					}
					player.EndAction (currAttackCooldown, cooldownMessage);
				}
			} else {
				attackTime = 0;
			}
		}
		if (meleeAttack) {
			meleeAttackElapsed += Time.deltaTime;
			handMover.ikActive = true;
			playerMove.handDist = meleeAttackReach * Mathf.Sin (Mathf.PI * (meleeAttackElapsed / meleeAttackAnim));
			if (meleeAttackElapsed >= meleeAttackAnim) {
				playerMove.handDist = baseHandDist;
				handMover.ikActive = GetComponent<Inventory>().IsHoldingItem();
				meleeAttack = false;
				meleeAttackElapsed = 0;
			}
		}
	}
}
