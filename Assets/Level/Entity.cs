using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Entity: De la que extienden todos los objetos en nivel controlados
/// por Level.cs
/// </summary>
public class Entity : MonoBehaviour {

	public static Level level;

	public string Name = "Unknown";
	public string description = "";
	public int actionsPerTurn = 1;
	public int actionsLeft;
	public int team = 0;
	public Color color = Color.white;
	public Spot spot; //Donde esta

	void OnMouseUp() {
		level.Select (this);
	}


	public virtual void Move(Entity obj, Vector3 newPos, float duration, AnimationCurve curve, Action endAction = null) {
		if (duration == 0f) {
			obj.transform.position = newPos;
		} else {
			StartCoroutine (MoveRoutine (obj, newPos, duration, curve, endAction));
		}
	}
	protected virtual IEnumerator MoveRoutine(Entity obj, Vector3 newPos, float duration, AnimationCurve curve, Action endAction = null) {
		float t = 0f;
		Vector3 startPos = obj.transform.position;
		while (t < duration) {
			yield return null;
			t += Time.deltaTime;
			obj.transform.position = Vector3.Lerp(startPos, newPos, curve.Evaluate(t/duration));
		}
		obj.transform.position = newPos;
		spot.Entered ();
		if (endAction != null)
			endAction ();
	}

	public delegate void EntityEvent();
	public event EntityEvent OnSelect;
	public void Select() { if(OnSelect!=null) OnSelect (); }
	public event EntityEvent OnUnselect;
	public void Unselect() { if(OnUnselect!=null) OnUnselect (); }
}
