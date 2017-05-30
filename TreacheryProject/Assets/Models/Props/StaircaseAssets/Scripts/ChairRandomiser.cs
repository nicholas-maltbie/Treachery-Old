using UnityEngine;
using System.Collections;

public class ChairRandomiser : MonoBehaviour {
	
	public void DestroyAllTraces(){
		var randomDestroy = Random.Range(0,2);
		var rotationVariation = Random.Range(-20,20);
		if(randomDestroy == 0){
			DestroyImmediate(this.transform.gameObject);	
		}else{
			transform.Rotate(Vector3.up * rotationVariation);
		}
	}
}
