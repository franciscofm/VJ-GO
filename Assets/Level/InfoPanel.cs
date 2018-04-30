using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public RectTransform panelRect;
	public Image backgroundImage;
	Rect panelInfoRect;
	RectTransform[] rects;
	public Text nameText;
	public Text actionsText;
	public Text descriptionText;
	public AspectRatioFitter aspectRatioFitter;

	public static Vector2 size;
	public static Vector3 offset;

	public void MoveAnchors() {
		float height = panelRect.rect.height;
		rects = new RectTransform[panelRect.childCount];
		float childsTotalHeight = 0f;
		panelRect.sizeDelta = Vector2.zero;
		for (int i = 0; i < rects.Length; ++i) {
			rects [i] = panelRect.GetChild (i).GetComponent<RectTransform> ();
			childsTotalHeight += rects [i].rect.height;
		}
		for (int i = 0; i < rects.Length; ++i) {
			float r = rects [i].rect.height / childsTotalHeight;
			float r2 = rects [i].rect.width / rects [i].rect.height;
			rects [i].sizeDelta = new Vector2 (rects [i].rect.height * r2, r * height);
		}
		panelInfoRect = panelRect.rect;
		Debug.Log (panelInfoRect.size);
	}

	public void Show(Entity entity) {
		aspectRatioFitter.enabled = false;
		nameText.text = entity.Name;
		actionsText.text = "AL: " + entity.actionsLeft;
		descriptionText.text = entity.description;
		Color color = entity.color;
		color.a = 0.3f;
		backgroundImage.color = color;
		Camera cam = Camera.main;
		Vector3 pos = cam.WorldToScreenPoint (entity.transform.position);
		transform.position = pos + 0.5f * offset;
		panelInfoRect.size = Vector2.zero;
		//StartCoroutine (ShowRoutine ());
	}
	IEnumerator ShowRoutine() {
		float t = 0f;
		float d = 1f;
		while (t < d) {
			yield return null;
			t += Time.deltaTime;
			float r = t / d;
			panelRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, size.x * r);
			panelRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, size.y * r);
		}
		aspectRatioFitter.enabled = true;
	}

	public void Hide() {
		
		gameObject.SetActive (false);
	}
}
