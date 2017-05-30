using UnityEngine;
using System.Collections;

/// <summary>
/// Make thunder and lightning sounds.
/// </summary>
[RequireComponent(typeof (AudioSource))]
[RequireComponent(typeof (Light))]
public class LightningThunder : MonoBehaviour {
	/// <summary>
	/// The audio source.
	/// </summary>
	private AudioSource audioSource;
	/// <summary>
	/// The light source.
	/// </summary>
	private Light lightSource;
	/// <summary>
	/// Different thunder and lightning sounds.
	/// </summary>
	public AudioClip[] sounds;
	/// <summary>
	/// The maximum gap between sounds.
	/// </summary>
	public float maxGap;
	/// <summary>
	/// The minimum gap between sounds.
	/// </summary>
	public float minGap;
	/// <summary>
	/// The length of the lightning.
	/// </summary>
	public float lightLength;
	/// <summary>
	/// Timing between lightning.
	/// </summary>
	private float elapsed, gap;
	
	void Start() {
		audioSource = GetComponent<AudioSource> ();
		lightSource = GetComponent<Light> ();
		gap = Random.Range(maxGap, minGap);
		lightSource.intensity = 0;
	}

	void Update () {
		if (!audioSource.isPlaying && sounds.Length > 0 && elapsed >= gap) {
			audioSource.clip = sounds[(int)Random.Range(0, sounds.Length)];
			audioSource.Play ();
			gap = Random.Range(maxGap, minGap);
			elapsed = 0;
			lightSource.intensity = 100;
		}
		if(elapsed >= lightLength)
		{
			lightSource.intensity = 0;
		}
		elapsed += Time.deltaTime;
	}
}
