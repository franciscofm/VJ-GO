using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
	public static void DrawlLineAttached(Transform parent, Vector3 start, Vector3 end) {
		DrawLineAttachedMultiple (parent, new Vector3[] { start, end });
	}
	public static void DrawLineAttachedMultiple(Transform parent, Vector3[] points) {
		GameObject obj = new GameObject ("Line of " + parent.name);
		obj.transform.parent = parent;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;

		LineRenderer line = obj.AddComponent<LineRenderer> ();
		line.useWorldSpace = false;
		line.positionCount = points.Length;
		line.widthMultiplier = 0.01f;
		line.SetPositions (points);
	}
}
