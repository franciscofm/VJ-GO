using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public bool blocked;
	public bool locked;

	public Transform target;
	public float zoomScale = 0;
	public float offsetNear = 1f;
	public float offsetFar = 6f;
	public Vector3 cameraRotation = new Vector3 (0f, 1f, 1f);

	public void FollowPlayer(Transform target) {
		this.target = target;
	}
	public void ShowMap(List<Vector3> positions, float zoomScale) {
		StartCoroutine (ShowMapRoutine (positions));
	}
	IEnumerator ShowMapRoutine(List<Vector3> positions) {
		for (int i = 0; i < positions.Count; ++i) {
			Vector3 pos = transform.position;
			float t = 0f;
			float d = Values.Camera.ShowSpotToSpot;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				transform.position = Vector3.Lerp (pos, positions [i], t / d);
			}
		}
		//Go to first player
	}

	public void FreeCamera() {
		locked = false;
	}
	public void LockCamera() {
		locked = true;
	}

	void Update() {
		if (!locked) return;
		Follow ();
	}
	void Follow() {
		Vector3 position = ((offsetFar - offsetNear) * zoomScale / Values.Camera.ZoomMax + offsetNear) * cameraRotation;
		position += target.position;
		transform.position = Vector3.Lerp (transform.position, position, Time.deltaTime * Values.Camera.Speed);
	}
}
