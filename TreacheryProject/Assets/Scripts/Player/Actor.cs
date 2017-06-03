using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An actor interacts with interactable objects.
/// </summary>
public class Actor : MonoBehaviour {

	private Interactable interactable;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		/*if (isLocalPlayer) {
			RaycastHit hit;
			if(Physics.SphereCast(GetCameraPosition(), .5f, GetCameraDirection().normalized, out hit, Mathf.Infinity, 1 << 8) && hit.collider.gameObject.GetComponent<Interactable>() != null && 
				hit.distance <= hit.collider.gameObject.GetComponent<Interactable>().useDistance)
			{
				CanInteract(hit.collider.gameObject);
			} else {
				CannotInteract();
			}
		}*/
	}
}
