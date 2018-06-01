using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio {

	public class Sound : MonoBehaviour {

		public AudioSource source;
		public AudioClip clip;
		public bool loop;

		IEnumerator routine;

		public void Init(AudioClip clip, bool loop, float volume, Vector3 position) {
			this.clip = clip;
			this.loop = loop;

			source.clip = clip;
			source.volume = volume;

			transform.position = position;

			source.Play ();
		}
		public void Init(AudioClip clip, bool loop, float volume, Transform parent) {
			this.clip = clip;
			this.loop = loop;

			source.clip = clip;
			source.volume = volume;

			transform.parent = parent;
			transform.localPosition = Vector3.zero;

			source.Play ();
		}

		public void Stop() {
			source.Stop ();
			Destroy(gameObject);
		}
	}

}