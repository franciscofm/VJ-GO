using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfo : MonoBehaviour {

	new public Text name;
	public GameObject death;
	public Image image;
	public Image health;
	public Image actions;
	CanvasGroup group;

	// Use this for initialization
	public void Init (string name, Sprite image) {
		group = GetComponent<CanvasGroup> ();
		death.SetActive (false);
		this.name.text = name;
		this.image.sprite = image;
	}
	public void Select() {
		StartCoroutine (Routine (Utils.EntityInfo.Aplha, 1f));
	}
	public void Unselect() {
		StartCoroutine (Routine (1f, Utils.EntityInfo.Aplha));
	}
	IEnumerator Routine(float fr, float to) {
		float t = 0f;
		float d = Utils.EntityInfo.Transition;
		while (t < d) {
			yield return null;
			t += Time.deltaTime;
			group.alpha = Mathf.Lerp (fr, to, t / d);
		}
	}

	public void SetHealth(float value) {
		health.fillAmount = value;
		if (value == 0f)
			Kill ();
	}
	public void SetActions(float value) {
		actions.fillAmount = value;
	}
	public void Kill() {
		death.SetActive (true);
		Unselect ();
	}
}
