using UnityEngine;
using System.Collections;

/// <summary>
/// Script to play multiple music sounds in order while someone is standing in the collider.
/// </summary>
[RequireComponent(typeof (AudioSource))]
[RequireComponent(typeof (Collider))]
public class MusicScript : MonoBehaviour {
	/// <summary>
	/// Sounds to play.
	/// </summary>
	public AudioClip[] songs;
	/// <summary>
	/// Current song being played.
	/// </summary>
	private int song;
	/// <summary>
	/// The audio source.
	/// </summary>
	private AudioSource audioSource;

	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			if(!audioSource.isPlaying)
			{
				audioSource.clip = songs[song];
				audioSource.Play();
				song++;
				song %= songs.Length;
			}
		}
	}

	void Start () {
		this.audioSource = GetComponent<AudioSource> ();
	}
}
