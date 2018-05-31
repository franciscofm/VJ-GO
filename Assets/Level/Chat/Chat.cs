using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Chat : MonoBehaviour {
	public GameObject clickPanel;
	[Header("Panel")]
	public RectTransform panel;
	public RectTransform panelOutside;
	Vector2 panelHiddenPos, panelShownPos;
	[Header("Images")]
	public RectTransform leftImageRect;
	public RectTransform rightImageRect;
	public RectTransform leftOutside, rightOutside;
	public RectTransform leftShown, rightShown;
	Vector2 leftImageOutsidePos, rightImageOutsidePos;
	Vector2 leftImageHiddenPos, rightImageHiddenPos;
	Vector2 leftImageShownPos, rightImageShownPos;
	Image leftImage, rightImage;
	[Header("Text")]
	public Text text;

	bool initialized = false;

	void Start() {
		leftImage = leftImageRect.GetComponent<Image> ();
		rightImage = rightImageRect.GetComponent<Image> ();
		leftImage.preserveAspect = true;
		rightImage.preserveAspect = true;

		panelShownPos = panel.transform.position;
		panelHiddenPos = panelOutside.position;

		leftImageOutsidePos = leftOutside.position;
		rightImageOutsidePos = rightOutside.position;
		leftImageShownPos = leftShown.position;
		rightImageShownPos = rightShown.position;
		leftImageHiddenPos = leftImageRect.position;
		leftImageHiddenPos.y -= panel.rect.height;
		rightImageHiddenPos = rightImageRect.position;
		rightImageHiddenPos.y -= panel.rect.height;

		leftImageRect.position = leftImageHiddenPos;
		rightImageRect.position = rightImageHiddenPos;
		panel.position = panelHiddenPos;

		initialized = true;
	}

	List<Phrase> dialog;
	int i;
	int leftId, rightId;
	bool focusLeft = true;
	public void Init(List<Phrase> dialog, Action EndChatCallback = null) {
		if (initialized)
			InitCode (dialog, EndChatCallback);
		else
			StartCoroutine (InitRoutine (dialog, EndChatCallback));
	}
	IEnumerator InitRoutine(List<Phrase> dialog, Action EndChatCallback = null) {
		yield return new WaitForEndOfFrame ();
		InitCode (dialog, EndChatCallback);
	}
	void InitCode(List<Phrase> dialog, Action EndChatCallback = null) {
		this.dialog = dialog;
		i = -1;
		leftId = rightId = -1;
		StartCoroutine(
			MoveRoutine(
				panel, 
				panelHiddenPos,
				panelShownPos, 
				Values.UI.Chat.ImageTransition,
				delegate {
					clickPanel.SetActive (true);
					Next();
				}
			));
	}

	IEnumerator routine = null;
	string textToWrite;
	public void Write(string text) {
		textToWrite = text;
		StartCoroutine (routine = WriteRoutine ());
	}
	IEnumerator WriteRoutine() {
		CustomYieldInstruction wait = new WaitForSecondsRealtime (Values.UI.Chat.TimePerCharacter);
		this.text.text = "";
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
	IEnumerator MoveRoutine(RectTransform rect, Vector2 start, Vector2 end, float duration, Action EndCallback = null) {
		float t = 0f;
		rect.position = start;
		while (t < duration) {
			yield return null;
			t += Time.deltaTime;
			rect.position = Vector2.Lerp (start, end, t / duration);
		}
		if (EndCallback != null)
			EndCallback ();
	}
	IEnumerator ScaleRoutine(RectTransform rect, float start, float end, float duration, Action EndCallback = null) {
		float t = 0f;
		rect.localScale = start * Vector3.one;
		while (t < duration) {
			yield return null;
			t += Time.deltaTime;
			rect.localScale = Mathf.Lerp (start, end, t / duration) * Vector3.one;
		}
		if (EndCallback != null)
			EndCallback ();
	}
	void Next() {
		++i;
		if (i < dialog.Count) {
			Write (dialog [i].text);
			if (dialog [i].position == Position.Left) { //lado izq
				if (!focusLeft) {
					StartCoroutine(ScaleRoutine(rightImageRect, 1f, Values.UI.Chat.ImageScale, Values.UI.Chat.ImageTransition));
					StartCoroutine(ScaleRoutine(leftImageRect, Values.UI.Chat.ImageScale, 1f, Values.UI.Chat.ImageTransition));
				}
				if (leftId == -1) { //aparece ese lado por primera vez
					leftImage.sprite = dialog [i].image;
					StartCoroutine(
						MoveRoutine(
							leftImageRect, 
							leftImageOutsidePos, 
							leftImageShownPos,
							Values.UI.Chat.ImageTransition
						));
				} else if (leftId == dialog [i].id) { //mismo pj dif imagen
					leftImage.sprite = dialog [i].image;
				} else { //cambio de pj
					StartCoroutine(
						ScaleRoutine(
							leftImageRect, 
							1f, Values.UI.Chat.ImageScale, 
							Values.UI.Chat.ImageTransition,
							delegate {
								leftImage.sprite = dialog [i].image;
								StartCoroutine(
									ScaleRoutine(
										leftImageRect, 
										Values.UI.Chat.ImageScale, 1f, 
										Values.UI.Chat.ImageTransition));
							}
						));
				}
				leftId = dialog [i].id;
				focusLeft = true;
			} else { //lado der
				if (focusLeft) {
					StartCoroutine(ScaleRoutine(leftImageRect, 1f, Values.UI.Chat.ImageScale, Values.UI.Chat.ImageTransition));
					StartCoroutine(ScaleRoutine(rightImageRect, Values.UI.Chat.ImageScale, 1f, Values.UI.Chat.ImageTransition));
				}
				if (rightId == -1) { //aparece ese lado por primera vez
					rightImage.sprite = dialog [i].image;
					StartCoroutine(
						MoveRoutine(
							rightImageRect, 
							rightImageOutsidePos, 
							rightImageShownPos,
							Values.UI.Chat.ImageTransition
						));
				} else if (rightId == dialog [i].id) { //mismo pj dif imagen
					rightImage.sprite = dialog [i].image;
				} else { //cambio de pj
					StartCoroutine(
						ScaleRoutine(
							rightImageRect, 
							1f, Values.UI.Chat.ImageScale, 
							Values.UI.Chat.ImageTransition,
							delegate {
								rightImage.sprite = dialog [i].image;
								StartCoroutine(
									ScaleRoutine(
										rightImageRect, 
										Values.UI.Chat.ImageScale, 1f, 
										Values.UI.Chat.ImageTransition));
							}
						));
				}
				rightId = dialog [i].id;
				focusLeft = false;
			}

		} else 
			Finish ();
	}
	void Finish() {
		StopAllCoroutines ();
		if(leftId != -1)
			StartCoroutine(
				MoveRoutine(
					leftImageRect, 
					leftImageShownPos, 
					leftImageHiddenPos,
					Values.UI.Chat.ImageTransition,
					delegate {
						StartCoroutine(
							MoveRoutine(
								panel, 
								panelShownPos, 
								panelHiddenPos,
								Values.UI.Chat.ImageTransition,
								delegate {
									clickPanel.SetActive (false);
									text.text = "";
								}
							));
					}
				));
		if(rightId != -1)
			StartCoroutine(
				MoveRoutine(
					rightImageRect, 
					rightImageShownPos, 
					rightImageHiddenPos,
					Values.UI.Chat.ImageTransition,
					delegate {
						if(leftId != -1)
							StartCoroutine(
								MoveRoutine(
									panel, 
									panelShownPos, 
									panelHiddenPos,
									Values.UI.Chat.ImageTransition,
									delegate {
										clickPanel.SetActive (false);
										text.text = "";
									}
								));
					}
				));
	}
}
