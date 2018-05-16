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
	public Sprite image;
	public Spot spot; //Donde esta

	void OnMouseUp() {
		level.Select (this);
	}

	public virtual void Move(Entity entity, Spot spot, float duration, AnimationCurve curve, Action endAction = null) {
		if (duration == 0f) {
			entity.spot.Leave ();
			entity.transform.position = spot.transform.position + Vector3.up;
			entity.spot.occupied = false;
			entity.spot.occupation = null;
			entity.spot = spot;
			spot.occupied = true;
			spot.occupation = entity;
			spot.Entered ();
		} else {
			StartCoroutine (MoveRoutine (entity, spot, duration, curve, endAction));
		}
	}
	protected virtual IEnumerator MoveRoutine(Entity entity, Spot spot, float duration, AnimationCurve curve, Action endAction = null) {
		entity.spot.Leave ();
		entity.spot.occupied = false;
		entity.spot.occupation = null;
		entity.spot = null;

		float t = 0f;
		Vector3 startPos = entity.transform.position;
		while (t < duration) {
			yield return null;
			t += Time.deltaTime;
			entity.transform.position = Vector3.Lerp(startPos, spot.transform.position + Vector3.up, curve.Evaluate(t/duration));
		}

		entity.spot = spot;
		spot.occupied = true;
		spot.occupation = entity;
		spot.Entered ();
		if (endAction != null)
			endAction ();
	}

	public void GetActions() {

	}
	public void ShowActions() {

	}

	public delegate void EntityEvent();
	public event EntityEvent OnSelect;
	public void Select() { if(OnSelect!=null) OnSelect (); }
	public event EntityEvent OnUnselect;
	public void Unselect() { if(OnUnselect!=null) OnUnselect (); }
}
