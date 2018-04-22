using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio {

	public class ManagerSoundSource : MonoBehaviour {

		public AudioSource source;
		public AudioClip clip;
		public bool loop;

		IEnumerator routine;

		public List<ManagerSoundSource> reference;
		public int channel;

		public void Init(AudioClip clip, bool loop, int channel,
			List<ManagerSoundSource> reference) {
			this.clip = clip;
			this.loop = loop;
			this.channel = channel;
			this.reference = reference;

			source.clip = clip;
		}
		public void Play() {
			StartCoroutine (routine = WaitLoop ());
		}

		IEnumerator WaitLoop() {
			source.Play ();
			while (source.isPlaying) yield return null; //syncing
			if (loop)
				StartCoroutine (routine = WaitLoop ());
			else
				Stop (true);
		}

		/// <summary>
		/// Stop the specified CallManager. Set to true if called outside the ManagerSound.
		/// </summary>
		public void Stop(bool CallManager) {
			StopCoroutine (routine); //shouldnt be necessary, but just in case
			if (CallManager) {
				ManagerSound.DiscardSound (this);
				ManagerSound.HitNext (channel);
			}
			source.Stop ();
			Destroy(gameObject);
		}
	}

}