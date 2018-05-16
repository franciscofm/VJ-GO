using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Chat : MonoBehaviour {
	public RectTransform panel;
	public RectTransform leftImage;
	public RectTransform rightImage;
	public Text text;

	List<Phrase> dialog;
	int i;
	public void Init(List<Phrase> dialog, Action EndChatCallback = null) {
		this.dialog = dialog;
		i = 0;
		Next ();
	}

	IEnumerator routine = null;
	string textToWrite;
	public void Write(string text) {
		textToWrite = text;
		StartCoroutine (routine = WriteRoutine ());
	}
	IEnumerator WriteRoutine() {
		CustomYieldInstruction wait = new WaitForSecondsRealtime (Values.UI.Chat.TimePerCharacter);
		for (int i = 0; i < textToWrite.Length; ++i) {
			this.text.text = string.Concat (this.text.text, textToWrite [i]);
			yield return wait;
		}
		routine = null;
	}

	public void PanelCallback() {
		if (routine != null) {
			StopCoroutine (routine);
			text.text = textToWrite;
		} else {
			Next ();
		}
	}
	void Next() {

	}
}
