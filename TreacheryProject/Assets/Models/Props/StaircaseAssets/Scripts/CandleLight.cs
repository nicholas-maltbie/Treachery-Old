using UnityEngine;
using System.Collections;

public class CandleLight : MonoBehaviour {

	public float bases = 2.0f; // start
	public float amplitude = 0.1f; // amplitude of the wave
	private float phase = 0.0f;
	public float frequency = 2.4f; // cycle frequency per second
	private Color originalColor;
	
	void Start () {	
		phase = Random.Range(0.0f,1.0f);
	    originalColor = this.transform.GetComponent<Light>().color;
	}
	void Update () {
	  this.transform.GetComponent<Light>().color = originalColor * EvalWave();
	}
	public float EvalWave () {
		var x  = (Time.time + phase+ Random.Range(0.0f,0.1f))*frequency;
		var y = new float();
		x = x - Mathf.Floor(x); // normalized value (0..1)
		y = Mathf.Sin(x*2*Mathf.PI);
		var backToColor = (y*amplitude)+bases; 
		return backToColor;
	}
}
