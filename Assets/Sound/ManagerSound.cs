using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//############################
//##### GESTOR DE AUDIO ######
//############################

namespace Audio {

	public static class ManagerSound {

		public static GameObject Source;

		//Volume
		static float VolumeGlobal = 1f;
		static float VolumeMusic = 1f;
		static float VolumeEffects = 1f;
		static float VolumeNarrations = 1f;

		public enum Type { Effect, Music, Narration }

		public static void Init(GameObject sourceModel) {
			Source = sourceModel;
		}
		public static Sound PlaySound(AudioClip clip, Transform parent, bool looped = false, Type type = Type.Effect) {

			if (clip == null) Error ("clip is NULL.");

			//Type and list reference
			float volume = VolumeGlobal;
			switch (type) {
			case Type.Effect:
				volume *= VolumeEffects;
				break;
			case Type.Music:
				volume *= VolumeMusic;
				break;
			case Type.Narration:
				volume += VolumeNarrations;
				break;
			}

			//Source creation
			GameObject source = GameObject.Instantiate (Source);
			Sound sound = source.GetComponent<Sound>();
			sound.Init (clip, looped, volume, parent);

			return sound;
		}
		public static Sound PlaySound(AudioClip clip, Vector3 position, bool looped = false, Type type = Type.Effect) {

			if (clip == null) Error ("clip is NULL.");

			//Type and list reference
			float volume = VolumeGlobal;
			switch (type) {
			case Type.Effect:
				volume *= VolumeEffects;
				break;
			case Type.Music:
				volume *= VolumeMusic;
				break;
			case Type.Narration:
				volume += VolumeNarrations;
				break;
			}

			//Source creation
			GameObject source = GameObject.Instantiate (Source);
			Sound sound = source.GetComponent<Sound>();
			sound.Init (clip, looped, volume, position);

			return sound;
		}

		static float ClampedVolume(float volume) {
			if (volume < 0f) return 0f;
			if (volume <= 1f) return volume;
			if (volume < 100f) return volume / 100f;
			return 1f;
		}
		static void Error(string msg) {
			Debug.Log ("ManagerSound error: " + msg);
		}

	}

}