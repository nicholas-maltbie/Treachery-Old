using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandMove : MonoBehaviour {

	public Transform handPos;
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
			if (ikActive) {
				//Set look weight (speed)
				anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
				anim.SetIKRotationWeight (AvatarIKGoal.RightHand, 1);
				//Set look target at the look object
				if (inv.held == null) {
					anim.SetIKPosition (AvatarIKGoal.RightHand, handPos.position);  
					anim.SetIKRotation (AvatarIKGoal.RightHand, handPos.rotation); 
				} else {
					//anim.SetIKPosition (AvatarIKGoal.RightHand, handPos.position);  
					anim.SetIKPosition (AvatarIKGoal.RightHand, inv.held.GetComponent<Item>().holdPos.position);  
					anim.SetIKRotation (AvatarIKGoal.RightHand, handPos.rotation); 
				}
			} else {
				anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 0);
				anim.SetIKRotationWeight (AvatarIKGoal.RightHand, 0);
			}
		}
	}
}
