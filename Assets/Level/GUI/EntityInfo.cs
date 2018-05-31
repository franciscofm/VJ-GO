using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfo : MonoBehaviour {

	new public Text name;
	public GameObject death;
	public Image image;
	public Transform actionsParent;
	public GameObject action;

	CanvasGroup group;
	List<GameObject> totalActions;

	// Use this for initialization
	public void Init (string name, Sprite image, int actions) {
		group = GetComponent<CanvasGroup> ();
		death.SetActive (false);
		group.alpha = Values.UI.EntityInfo.Aplha;
		this.name.text = name;
		this.image.sprite = image;
		SetTotalActions (actions);
	}
	public void Select() {
		StartCoroutine (Routine (Values.UI.EntityInfo.Aplha, 1f));
	}
	public void Unselect() {
		StartCoroutine (Routine (1f, Values.UI.EntityInfo.Aplha));
	}
	IEnumerator Routine(float fr, float to) {
		float t = 0f;
		float d = Values.UI.EntityInfo.Transition;
		while (t < d) {
			yield return null;
			t += Time.deltaTime;
			group.alpha = Mathf.Lerp (fr, to, t / d);
		}
	}

	public void SetTotalActions(int a) {
		if (totalActions == null)
			totalActions = new List<GameObject> ();
		else {
			while (totalActions.Count > 0) {
				Destroy (totalActions [0]);
				totalActions.RemoveAt (0);
			}
		}
		for (int i = 0; i < a; ++i) {
			totalActions.Add (Instantiate (action, actionsParent));
		}
	}
	public void UseAction() {
		for (int i = 0; i < totalActions.Count; ++i)
			if (totalActions [i].activeSelf) {
				totalActions [i].SetActive (false);
				return;
			}
	}
	public void RecoverActions(int a) {
		int recovered = 0;
		for (int i = 0; i < totalActions.Count && recovered < a; ++i) {
			if (!totalActions [i].activeSelf) {
				totalActions [i].SetActive (true);
				++recovered;
			}
		}
	}
	public void Kill() {
		death.SetActive (true);
		Unselect ();
	}
}
