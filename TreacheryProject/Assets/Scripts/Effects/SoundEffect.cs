using UnityEngine;
using System.Collections;

/// <summary>
/// Plays sounds from a list in a random order.
/// </summary>
public class SoundEffect : MonoBehaviour {
	/// <summary>
	/// The audio source.
	/// </summary>
	private AudioSource audioSource;
	/// <summary>
	/// The sounds that can be played.
	/// </summary>
	public AudioClip[] sounds;
	/// <summary>
	/// The min and max gap between sound effects.
	/// </summary>
	public float maxGap, minGap;
	/// <summary>
	/// Timing between sound effects.
	/// </summary>
	private float elapsed, gap;

	void Start() {
		audioSource = GetComponent<AudioSource> ();
		elapsed = maxGap;
	}

	// Update is called once per frame
	void Update () {
		if (!audioSource.isPlaying && sounds.Length > 0 && elapsed >= gap) {
			audioSource.clip = sounds[(int)Random.Range(0, sounds.Length)];
			audioSource.Play ();
			gap = Random.Range(maxGap, minGap);
			elapsed = 0;
		}
		else
			elapsed += Time.deltaTime;
	}
}
