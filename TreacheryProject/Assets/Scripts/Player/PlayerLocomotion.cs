/**
 * Copyright (C) 2017 Nicholas Maltbie
 * GNU GPL V3
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * A Player locomotion Controller will take a player's current 
 * movement and use this to control the character in game and the
 * corresponding animation using the animation controller.
 * 
 */
public class PlayerLocomotionController : MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Start () {
		//Get the animator from the main class.
		anim = gameObject.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
