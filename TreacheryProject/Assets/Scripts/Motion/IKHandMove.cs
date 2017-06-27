using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandMove : MonoBehaviour {

	public Transform handPos;
	public bool active = true;
	public bool ikActive = true;

	public Inventory inv;
	public Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	void OnAnimatorIK()
	{
		if(GetComponent<Animator>()) {
			//if the IK is active, set the position and rotation directly to the goal. 
			if (ikActive && active) {
				//Set look weight (speed)
				anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
				anim.SetIKRotationWeight (AvatarIKGoal.RightHand, 1);
				//Set look target at the look object
				anim.SetIKPosition (AvatarIKGoal.RightHand, handPos.position);  
				anim.SetIKRotation (AvatarIKGoal.RightHand, handPos.rotation);  
			} else {
				anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 0);
				anim.SetIKRotationWeight (AvatarIKGoal.RightHand, 0);
			}
		}
	}
}
