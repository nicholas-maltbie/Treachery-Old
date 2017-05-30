using UnityEngine;
using System.Collections;

public class RotateMe : MonoBehaviour {
	public float rotationSpeed;
	public bool AxisX = true;
	public bool AxisY = false;
	public bool AxisZ = false;

	// Update is called once per frame
	void Update () {
	
			if(AxisX == true){
				transform.Rotate(Vector3.right * Time.deltaTime * rotationSpeed);
			}
			if(AxisY == true){
				transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
			}
			if(AxisZ == true){
				transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
			}
		
	}
}
