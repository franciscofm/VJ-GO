using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public RectTransform panelRect;
	public Image backgroundImage;
	RectTransform[] rects;
	public Text nameText;
	public Text actionsText;
	public Text descriptionText;
	public static Vector3 offset;

	public void Init() {
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
	}

	public void Show(Entity entity) {
		nameText.text = entity.Name;
		actionsText.text = "AL: " + entity.actionsLeft;
		descriptionText.text = entity.description;
		Color color = entity.color;
		color.a = 0.3f;
		backgroundImage.color = color;
		Camera cam = Camera.main;
		Vector3 pos = cam.WorldToScreenPoint (entity.transform.position);
		transform.position = pos + 0.5f * offset;
	}

	public void Hide() {
		
		gameObject.SetActive (false);
	}
}
