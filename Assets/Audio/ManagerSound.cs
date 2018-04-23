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
		static float VOLUME_GLOBAL = 1f;
		static float VOLUME_BACKGROUND = 1f;
		static float VOLUME_EFFECTS = 1f;
		static float VOLUME_NARRATIONS = 1f;
		public static float GlobalVolume {
			get { return VOLUME_GLOBAL; }
			set {
				VOLUME_GLOBAL = ClampedVolume(GlobalVolume);
				ChangeMasterVolume ();
			}
		}
		public static float BackgroundVolume {
			get { return VOLUME_BACKGROUND; }
			set {
				VOLUME_BACKGROUND = ClampedVolume(BackgroundVolume);
				ChangeTypeVolume (BackgroundsL,VOLUME_BACKGROUND);
			}
		}
		public static float EffectsVolume {
			get { return VOLUME_EFFECTS; }
			set {
				VOLUME_EFFECTS = ClampedVolume(EffectsVolume);
				ChangeTypeVolume (EffectsL,VOLUME_EFFECTS);
			}
		}
		public static float NarrationsVolume {
			get { return VOLUME_NARRATIONS; }
			set {
				VOLUME_NARRATIONS = ClampedVolume(NarrationsVolume);
				ChangeTypeVolume (NarrationsL,VOLUME_NARRATIONS);
			}
		}

		//Availability
		public static int USED_SOURCES { get ; private set; }
		public const int MAX_SOURCES = 32;
		public const int MAX_CHANNELS = 12;

		//Channel position
		public const int CH_BACKGROUND = 0;
		public static int CH_FIRST_FREE { get ; private set; }

		//Channel queue flags
		public const int FLAG_PRIORITY_LOW = 0x01;
		public const int FLAG_PRIORITY_HIGH = 0x02;
		public const int FLAG_CLEAR_CHANNEL = 0x04;

		//Clip flags
		public const int TYPE_BACKGROUND = 0x01;
		public const int TYPE_EFFECT = 0x02;
		public const int TYPE_NARRATION = 0x04;
		//0x08, 0x10, 0x20, 0x40

		static AudioClip[] Narrations;
		static AudioClip[] Effects;

		static List<ManagerSoundSource> EffectsL;
		static List<ManagerSoundSource> NarrationsL;
		static List<ManagerSoundSource> BackgroundsL;

		static List<ManagerSoundSource>[] Channels;

		public static void Init(GameObject sourceModel) {
			Source = sourceModel;

			CH_FIRST_FREE = 1;
			USED_SOURCES = 0;
			Channels = new List<ManagerSoundSource>[MAX_CHANNELS];
			for (int i = 0; i < MAX_CHANNELS; ++i)
				Channels [i] = new List<ManagerSoundSource> ();
			EffectsL = new List<ManagerSoundSource> ();
			NarrationsL = new List<ManagerSoundSource> ();
			BackgroundsL = new List<ManagerSoundSource> ();
		}
		public static void LoadNarrations(AudioClip[] narrations) { //pass to dynamic loading
			Narrations = narrations;
		}

	//	public static void unloadNarrations(){
	//		for (int i = 0; i < narrations.Length; ++i)
	//			Resources.UnloadAsset (narrations [i]);
	//	}
	//	public static void loadNarrations(){
	//		string language = ManagerUser.UserInfo.Language;
	//		narrations = Resources.LoadAll<AudioClip>("Narrations" + System.IO.Path.DirectorySeparatorChar + language);
	//	}


		//### NEW SECTION
		public static ManagerSoundSource PlayNarration(AudioClip narration, int channel, int priority) {
			return PlaySound (FindNarration(narration), false, channel, priority, TYPE_NARRATION);
		}
		public static ManagerSoundSource PlayEffect(AudioClip effect) {
			return PlaySound (effect, false, CH_FIRST_FREE);
		}
		public static ManagerSoundSource PlayBackground(AudioClip background) {
			return PlaySound (background, false, CH_BACKGROUND, FLAG_CLEAR_CHANNEL, TYPE_BACKGROUND);
		}
		public static ManagerSoundSource PlaySound(
													AudioClip clip, 
													bool looped = false, 
													int channel = 1, 
													int priority = FLAG_PRIORITY_LOW, 
													int type = TYPE_EFFECT) {
//			Debug.Log ("Play sound call; clip:" + clip.name 
//				+ ", looped:" + looped 
//				+ "channel:" + channel 
//				+ ", priority:" + priority 
//				+ ", type:" + type);
			//Error managment
			if (clip == null) Error ("clip is NULL.");
			if (channel < 0 || MAX_CHANNELS <= channel) Error ("channel is (<0) or (>MAX_CHANNELS).");
			if (USED_SOURCES >= MAX_SOURCES) Error ("max number of sources reached.");

			//Type and list reference
			List<ManagerSoundSource> reference = null;
			switch (type) {
			case TYPE_EFFECT:
				reference = EffectsL;
				break;
			case TYPE_BACKGROUND:
				reference = BackgroundsL;
				break;
			case TYPE_NARRATION:
				reference = NarrationsL;
				break;
			}

			//Source creation
			GameObject source = GameObject.Instantiate (Source);
			ManagerSoundSource mss = source.GetComponent<ManagerSoundSource>();
			mss.Init (clip, looped, channel, reference);
			reference.Add (mss);

			//Queue placement
			//Clear & priority
			if ((priority & FLAG_CLEAR_CHANNEL) == FLAG_CLEAR_CHANNEL) {
				StopChannel (channel);
				Channels [channel].Add (mss);
				mss.Play ();
			} else {
				if ((priority & FLAG_PRIORITY_LOW) == FLAG_PRIORITY_LOW) {
					Channels [channel].Add (mss);
					if (Channels [channel].Count == 1)
						mss.Play ();
				}else if ((priority & FLAG_PRIORITY_HIGH) == FLAG_PRIORITY_HIGH) {
					Channels [channel].Insert (0, mss);
					mss.Play ();
				}
			}
			FindFirstFree ();

			++USED_SOURCES;
			return mss;
		}


		public static void HitNext(int channel) {
			int i = 0;
			while (i < Channels [channel].Count) {
				if (Channels [channel] [i].source.isPlaying)
					++i;
				else {
					Channels [channel] [i].Play ();
					return;
				}
			}
		}

		public static void DiscardSound(ManagerSoundSource mss) { //Called only by ManagerSoundSource
			Channels [mss.channel].Remove (mss);
			mss.reference.Remove (mss);
			--USED_SOURCES;
		}
		public static void StopChannel(int channel) {
			List<ManagerSoundSource> list = Channels [channel];
			while (list.Count > 0) 
				list [0].Stop (true);
		}
		public static void StopType(int type = TYPE_EFFECT) {
			List<ManagerSoundSource> list = null;
			switch (type) {
			case TYPE_EFFECT:
				list = EffectsL;
				break;
			case TYPE_BACKGROUND:
				list = BackgroundsL;
				break;
			case TYPE_NARRATION:
				list = NarrationsL;
				break;
			}
			while (list.Count > 0) 
				list [0].Stop (true);
		}

		static float ClampedVolume(float volume) {
			if (volume < 0f) return 0f;
			if (volume <= 1f) return volume;
			if (volume < 100f) return volume / 100f;
			return 1f;
		}
		static void ChangeMasterVolume() {
			ChangeTypeVolume (BackgroundsL,VOLUME_BACKGROUND);
			ChangeTypeVolume (EffectsL,VOLUME_EFFECTS);
			ChangeTypeVolume (NarrationsL,VOLUME_NARRATIONS);
		}
		static void ChangeTypeVolume(List<ManagerSoundSource> list, float volume) {
			for (int i = 0; i < list.Count; ++i)
				list [i].source.volume = volume * VOLUME_GLOBAL;
		}

		static void FindFirstFree() {
			for (int i = 1; i < Channels.Length; ++i)
				if (Channels [i].Count == 0) {
					CH_FIRST_FREE = i;
					return;
				}
			CH_FIRST_FREE = -1;
		}
		static AudioClip FindNarration(AudioClip clip){
			for (int i = 0; i < Narrations.Length; ++i) {
				if (Narrations [i].name.Equals (clip.name))
					return Narrations [i];
			}
			return null;
		}

		static void Error(string msg) {
			Debug.Log ("ManagerSound error: " + msg);
		}

	}

}