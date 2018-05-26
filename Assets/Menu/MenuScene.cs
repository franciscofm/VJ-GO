using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene : MonoBehaviour {

	public AudioSource source;
	public AudioClip lightSound;
	void Start() {
		source.clip = lightSound;
	}
	public void PlayLightSound() {
		source.Play ();
	}
}
