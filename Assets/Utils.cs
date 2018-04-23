using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

	public static void CopyLineRenderer(LineRenderer original, LineRenderer copy) {
		copy.sharedMaterial = original.sharedMaterial;

		copy.widthCurve = original.widthCurve;
		copy.widthMultiplier = original.widthMultiplier;
		copy.startWidth = original.startWidth;
		copy.endWidth = original.endWidth;

		copy.colorGradient = original.colorGradient;
		copy.startColor = original.startColor;
		copy.endColor = original.endColor;

		copy.loop = original.loop;
		copy.alignment = original.alignment;
		copy.generateLightingData = original.generateLightingData;
		copy.numCapVertices = original.numCapVertices;
		copy.numCornerVertices = original.numCornerVertices;
		copy.textureMode = original.textureMode;
		copy.useWorldSpace = original.useWorldSpace;
	}

}
