using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class CandelLight : MonoBehaviour {

	public float variance = 0.06f;
	public float freq = 0.65f;
	public float chance = 0.1f;

	private Light source;
	private float normal;
	private float elapsed = 0f;

	// Use this for initialization
	void Start () {
		source = GetComponent<Light> ();
		normal = source.intensity;
		elapsed = Random.Range (0, freq * 100);
	}
	
	// Update is called once per frame
	void Update () {
		elapsed += Time.deltaTime;
		source.intensity = normal + 
			(Mathf.Cos ((elapsed + Random.Range (0, chance)) * Mathf.PI * 2 / freq) +
				Mathf.Sin ((elapsed + Random.Range (0, chance)) * Mathf.PI * 2 / freq))
			* variance;
	}
}
