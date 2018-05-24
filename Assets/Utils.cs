using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
	public static void DrawlLineAttached(Transform parent, Vector3 start, Vector3 end, Material mat = null, float width = 0.01f) {
		DrawLineAttachedMultiple (parent, new Vector3[] { start, end }, mat, width);
	}
	public static void DrawLineAttachedMultiple(Transform parent, Vector3[] points, Material mat, float width) {
		GameObject obj = new GameObject ("Line of " + parent.name);
		obj.transform.parent = parent;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		obj.transform.localScale = Vector3.one;

		LineRenderer line = obj.AddComponent<LineRenderer> ();
		line.useWorldSpace = false;
		line.positionCount = points.Length;
		line.widthMultiplier = width;
		line.SetPositions (points);
		line.material = mat;
	}
}
