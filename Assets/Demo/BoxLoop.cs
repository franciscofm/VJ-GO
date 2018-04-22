using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxLoop : MonoBehaviour {

	public ContentAssigner contr;
	bool loop = false;
	float animD = 0.4f;
	void OnMouseDown() {
		loop = !loop;
		contr.Loop (loop);
		ChangeRotation ();
	}
	void ChangeRotation() {
		StopAllCoroutines ();
		StartCoroutine (Rotate (loop ? 45f : 0f));
	}
	IEnumerator Rotate(float rot){
		Vector3 start = transform.localEulerAngles;
		Vector3 end = new Vector3 (rot, 0f, 0f);
		float t = 0;
		while (t < animD) {
			yield return null;
			t += Time.deltaTime;
			transform.localEulerAngles = Vector3.Lerp (start, end, t / animD);
		}
		transform.localEulerAngles = end;
	}
}
