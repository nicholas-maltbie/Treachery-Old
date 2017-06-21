using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This class adds a sound effect for every time the object collides with another collider.
/// </summary>
[RequireComponent(typeof (AudioSource))]
[RequireComponent(typeof (Collider))]
public class CollisionSound : NetworkBehaviour {
	/// <summary>
	/// Sounds that the object can make.
	/// </summary>
	public AudioClip[] sounds;
	/// <summary>
	/// The audio source.
	/// </summary>
	private AudioSource audioSource;
	/// <summary>
	/// Time in seconds since last sound was played.
	/// </summary>
	private float lastSound = 0;
	/// <summary>
	/// The minimum trigger velocity for a sound to play.
	/// </summary>
	public float minTrigger = .01f;
	/// <summary>
	/// The volume scalar.
	/// </summary>
	public float volumeScalar = 1;
	/// <summary>
	/// minimum delay between sounds for collisions.
	/// </summary>
	public float timeDelay = 0.2f;

	/// <summary>
	/// Whenever there is a collision, if it is above the min velocity, make a sound..
	/// </summary>
	/// <param name="col">Collision event.</param>
	void OnCollisionStay (Collision col)
	{
		if (isServer && col.relativeVelocity.magnitude >= minTrigger)
			RpcDoCollision (col.relativeVelocity.magnitude);
	}

	/// <summary>
	/// Whenever there is a collision, if it is above the min velocity, make a sound.
	/// </summary>
	/// <param name="col">Collision event.</param>
	void OnCollisionEnter (Collision col)
	{
		if (isServer && col.relativeVelocity.magnitude >= minTrigger)
			RpcDoCollision (col.relativeVelocity.magnitude);
	}

	/// <summary>
	/// Play collision sound on all the clients.
	/// </summary>
	/// <param name="strength">Strength.</param>
	[ClientRpc]
	public void RpcDoCollision(float strength)
	{
		if (strength >= minTrigger && lastSound > timeDelay) {
			audioSource.clip = sounds[Random.Range(0, sounds.Length)];
			audioSource.volume = volumeScalar  * Mathf.Log10(strength);
			audioSource.Play();
			lastSound = 0;
		}
	}
	
	void Start () {
		this.audioSource = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		lastSound += Time.deltaTime;
	}
}
