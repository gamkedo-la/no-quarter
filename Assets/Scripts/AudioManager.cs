using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager Instance;

	public AudioSource sfxPrefab;

	void Awake() {
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else Destroy(gameObject);
	}

	public AudioSource PlaySFX(AudioClip clip, GameObject objectToAttachTo, float volume = 1, float pitch = 1, float blend = 1f) {
		AudioSource freshAudioSource = Instantiate(sfxPrefab);
		freshAudioSource.transform.parent = transform;
		freshAudioSource.name = clip.name;

		freshAudioSource.clip = clip;
		freshAudioSource.volume = volume;
		freshAudioSource.pitch = pitch;
		freshAudioSource.spatialBlend = blend;
		freshAudioSource.Play();

		Destroy(freshAudioSource.gameObject, clip.length/pitch);

		return freshAudioSource;
	}

	public AudioSource PlaySFX(AudioClip clip, Vector3 positionToPlayAt, float volume = 1, float pitch = 1, float blend = 1f) {
		GameObject freshGameObject = new GameObject();
		freshGameObject.transform.position = positionToPlayAt;

		Destroy(freshGameObject, clip.length * pitch);
		return PlaySFX(clip, freshGameObject, volume, pitch, blend);
	}
}
