using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene : MonoBehaviour {

	public AudioSource source;
	public AudioClip lightSound;
	public float pitchMin = 0.8f;
	public float pitchMax = 1.2f;
	void Start() {
		source.clip = lightSound;
	}
	public void PlayLightSound() {
		source.Play ();
	}
}
