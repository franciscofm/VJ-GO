﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
	
	public class LevelUI : MonoBehaviour {
		public Image Image;
		public Text Name;
		public Text Description;
		public float completion;
		public Image Completion;
		public Text CompletionText;
		public string Scene;
		public CanvasGroup canvasGroup;
		public RectTransform rectTransform;

		public static Controller controller;

		public void SetValues(LevelUIEntry entry, float completion) {
			this.Scene = entry.Scene;
			SetValues (entry.Name, entry.Description, entry.Image, completion);
		}
		public void SetValues(string name, string description, Sprite image, float completion) {
			Name.text = name;
			Description.text = description;
			Image.sprite = image;
			this.completion = completion;
			Completion.fillAmount = completion;
			CompletionText.text = 100 * ((int)completion) + "%";
		}
		public void UpdateCompletion(float completion) {
			CompletionText.text = 100 * (int)completion + "%";
			Completion.fillAmount = completion;
		}

		public void FadeIn(float wait) {
			StartCoroutine (FadeRoutine (0f, 1f, Values.Menu.SelectLevel.LevelUIFadeIn, wait));
		}
		public void FadeOut() {
			StartCoroutine (FadeRoutine (1f, 0f, Values.Menu.SelectLevel.LevelUIFadeOut));
		}
		IEnumerator FadeRoutine(float a, float b, float d, float wait = 0f) {
			if (wait != 0f) yield return new WaitForSeconds (wait);
			float t = 0f;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				canvasGroup.alpha = Mathf.Lerp (a, b, t / d);
			}
		}

		public void MouseFocus() {
			int focusValue = (transform.parent.GetSiblingIndex() * 2) + transform.GetSiblingIndex();
			controller.MouseFocus (focusValue);
			Focus ();
		}
		public void Focus() {
			StartCoroutine (ScaleRoutine (1f, Values.Menu.SelectLevel.LevelUIScale));
		}
		public void Unfocus() {
			StartCoroutine (ScaleRoutine (Values.Menu.SelectLevel.LevelUIScale, 1f));
		}
		IEnumerator ScaleRoutine(float a, float b) {
			float t = 0f;
			float d = Values.Menu.SelectLevel.LevelUIScaleDuration;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				rectTransform.localScale = Mathf.Lerp (a, b, t / d) * Vector3.one;
			}
		}

		public void Select() {
			//Debug.Log ("Selected");
			controller.SelectLevel (this);
		}
	}
}