using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public bool blocked;
	public bool locked = true;
	public bool requested = false;
	public bool showing = false;
	public bool focusing = false;
	IEnumerator routine;

	Transform parent;
	public Transform target;
	public float zoomScale = 0;
	public Vector3 cameraRotation;
	public float offsetDiff;
	public float offsetNear;
	public float zoomMax;
	public float speed;

	void Awake() {
		parent = transform.parent;
	}
	void Start() {
		//parent = transform.parent;
		offsetDiff = Values.Camera.OffsetFar - Values.Camera.OffsetNear;
		zoomMax = Values.Camera.ZoomMax;
		offsetNear = Values.Camera.OffsetNear;
		speed = Values.Camera.Speed;
	}

	public void Follow(Transform target) {
		this.target = target;
		locked = true;
		focusing = false;
		showing = false;
	}
	public void Focus(Transform target, Action callback = null) {
		focusing = true;
		showing = false;
		locked = false;
		StartCoroutine (routine = FocusRoutine (target, callback));
	}
	public void ShowMap(List<Vector3> positions, Transform followUp, float zoomScale) {
		showing = true;
		StartCoroutine (routine = ShowMapRoutine (positions,followUp));
	}
	IEnumerator FocusRoutine(Transform target, Action callback = null) {
		Vector3 pos = parent.position;
		float t = 0f;
		float d = Values.Camera.FocusTime;
		while (t < d) {
			yield return null;
			t += Time.deltaTime;
			float distance = (offsetDiff * zoomScale / zoomMax) + offsetNear;
			Vector3 finalPos = target.position - (distance * cameraRotation);
			parent.position = Vector3.Lerp (pos, finalPos, t/d);
		}
		focusing = false;
		if (callback != null) callback ();
	}
	IEnumerator ShowMapRoutine(List<Vector3> positions, Transform followUp) {
		for (int i = 0; i < positions.Count; ++i) {
			Vector3 pos = parent.position;
			float t = 0f;
			float d = Values.Camera.ShowSpotToSpot;
			while (t < d) {
				yield return null; 
				t += Time.deltaTime;
				float distance = (offsetDiff * zoomScale / zoomMax) + offsetNear;
				Vector3 finalPos = positions[i] - (distance * cameraRotation);
				parent.position = Vector3.Lerp (pos, finalPos, t/d);
			}
		}
		showing = false;
		Focus(followUp);
	}

	public void FreeCamera() {
		locked = false;
	}
	public void LockCamera() {
		locked = true;
	}

	void Update() {
		if (showing || focusing) {
			return;
		} else {
			if (locked) {
				Follow ();
				if (routine != null) {
					StopCoroutine (routine);
					requested = false;
				}
			} else
				GetInput ();
		}
	}
	void Follow() {
		if (target == null) {
			locked = false;
			return;
		}
		requested = false;
		float distance = (offsetDiff * zoomScale / zoomMax) + offsetNear;
		Vector3 finalPos = target.position - (distance * cameraRotation);
		parent.position = Vector3.Lerp (parent.position, finalPos, Time.deltaTime * speed);
	}
	void GetInput() {
		if (Input.anyKey) {
			float x = Input.GetAxisRaw ("Horizontal");
			float z = Input.GetAxisRaw ("Vertical");

			parent.position += Time.deltaTime * Values.Camera.FreeSpeed * transform.TransformDirection (x, 0f, 0f);
			parent.position += Time.deltaTime * Values.Camera.FreeSpeed * parent.TransformDirection (0f, 0f, z);

			if (requested) {
				StopCoroutine (routine);
				requested = false;
			}
		} else {
			if (!requested) {
				StartCoroutine (routine = WaitRoutine ());
				requested = true;
			}
		}
	}
	IEnumerator WaitRoutine() {
		yield return new WaitForSeconds (Values.Camera.FreeRelocationTime);
		target = Level.instance.RequestTarget ();
	}
}
