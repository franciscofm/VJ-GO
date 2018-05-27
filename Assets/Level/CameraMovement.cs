using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public bool blocked;
	public bool locked = true;
	public bool requested = false;
	IEnumerator routine;

	Transform parent;
	public Transform target;
	public float zoomScale = 0;
	public Vector3 cameraRotation;

	void Start() {
		parent = transform.parent;
	}

	public void FollowPlayer(Transform target) {
		this.target = target;
		locked = true;
	}
	public void ShowMap(List<Vector3> positions, float zoomScale) {
		StartCoroutine (ShowMapRoutine (positions));
	}
	IEnumerator ShowMapRoutine(List<Vector3> positions) {
		for (int i = 0; i < positions.Count; ++i) {
			Vector3 pos = parent.position;
			float t = 0f;
			float d = Values.Camera.ShowSpotToSpot;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				parent.position = Vector3.Lerp (pos, positions [i], t / d);
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
		if (locked)
			Follow ();
		else 
			GetInput ();
	}
	void Follow() {
		if (target == null) {
			locked = false;
			return;
		}
		requested = false;
		float distance = ((Values.Camera.OffsetFar - Values.Camera.OffsetNear) * zoomScale / Values.Camera.ZoomMax) + Values.Camera.OffsetNear;
		Vector3 finalPos = target.position - (distance * cameraRotation);
		parent.position = Vector3.Lerp (parent.position, finalPos, Time.deltaTime * Values.Camera.Speed);
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
