using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {

	public InputField input;
	Level level;
	bool god;

	void Start() {
		level = Level.instance;
		god = false;
	}

	public virtual void ReadConsole() {
		Debug.Log (input.text);
		switch (input.text) {
		case "godmode":
			if (!god) {
				god = true;
				level.GodMode ();
			} else {
				god = false;
				level.NormalMode ();
			}
			break;
		case "skiplevel":
			level.SkipLevel ();
			break;
		case "h4h4l0l":
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
			break;
		}
		gameObject.SetActive (false);
	}
}
